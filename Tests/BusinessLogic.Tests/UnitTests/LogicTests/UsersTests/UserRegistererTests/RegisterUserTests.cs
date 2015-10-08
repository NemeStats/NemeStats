#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion
using System;
using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.UsersTests.UserRegistererTests
{
    [TestFixture]
    public class RegisterUserTests
    {
        private IFirstTimeAuthenticator firstTimeUserAuthenticatorMock;
        private IUserRegisterer userRegisterer;
        private IUserStore<ApplicationUser> userStoreMock;
        private IDataContext dataContextMock;
        private IAuthenticationManager authenticationManagerMock;
        private ApplicationSignInManager signInManagerMock;
        private ApplicationUserManager applicationUserManagerMock;
        private IDataProtectionProvider dataProtectionProviderMock;
        private INemeStatsEventTracker eventTrackerMock;
        private IGamingGroupInviteConsumer gamingGroupInviteConsumerMock;

        private NewUser newUser;
        private NewlyRegisteredUser expectedNewlyRegisteredUser;
        private const string applicationUserIdAfterSaving = "new application user Id";

        [SetUp]
        public void SetUp()
        {
            firstTimeUserAuthenticatorMock = MockRepository.GenerateMock<IFirstTimeAuthenticator>();
            userStoreMock = MockRepository.GenerateMock<IUserStore<ApplicationUser>>();
            var dataProtector = MockRepository.GenerateMock<IDataProtector>();
            dataProtectionProviderMock = MockRepository.GenerateMock<IDataProtectionProvider>();
            dataProtectionProviderMock.Expect(mock => mock.Create(Arg<string>.Is.Anything)).Return(dataProtector);
            applicationUserManagerMock = MockRepository.GenerateMock<ApplicationUserManager>(userStoreMock, dataProtectionProviderMock);
            authenticationManagerMock = MockRepository.GenerateMock<IAuthenticationManager>();
            signInManagerMock = MockRepository.GenerateMock<ApplicationSignInManager>(applicationUserManagerMock, authenticationManagerMock);
            dataContextMock = MockRepository.GenerateMock<IDataContext>();
            eventTrackerMock = MockRepository.GenerateMock<INemeStatsEventTracker>();
            gamingGroupInviteConsumerMock = MockRepository.GenerateMock<IGamingGroupInviteConsumer>();

            userRegisterer = new UserRegisterer(
                applicationUserManagerMock, 
                firstTimeUserAuthenticatorMock, 
                dataContextMock, 
                signInManagerMock,
                eventTrackerMock,
                gamingGroupInviteConsumerMock);

            Guid invitationId = Guid.NewGuid();
            const int PLAYER_ID = 1938;
            GamingGroupInvitation invitation = new GamingGroupInvitation
            {
                Id = invitationId,
                PlayerId = PLAYER_ID,
                GamingGroupId = 10
            };
            newUser = new NewUser()
            {
                UserName = "user name",
                EmailAddress = "the email",
                GamingGroupInvitationId = invitationId,
                Source = TransactionSource.WebApplication
            };
            expectedNewlyRegisteredUser = new NewlyRegisteredUser
            {
                GamingGroupId = invitation.GamingGroupId,
                GamingGroupName = "some awesome gaming group name",
                PlayerId = PLAYER_ID,
                PlayerName = "some awesome player name",
                UserId = applicationUserIdAfterSaving
            };
            IdentityResult result = IdentityResult.Success;

            dataContextMock.Expect(mock => mock.FindById<GamingGroupInvitation>(invitationId))
                           .Return(invitation);
            applicationUserManagerMock.Expect(mock => mock.CreateAsync(Arg<ApplicationUser>.Is.Anything, Arg<string>.Is.Anything))
                                      .Return(Task.FromResult(result))
                                      .WhenCalled(invocation => ((ApplicationUser)invocation.Arguments[0]).Id = applicationUserIdAfterSaving);
                                      
            signInManagerMock.Expect(mock => mock.SignInAsync(
                                                                          Arg<ApplicationUser>.Is.Anything,
                                                                          Arg<bool>.Is.Anything,
                                                                          Arg<bool>.Is.Anything))
                                         
                                         .Return(Task.FromResult(-1));
            gamingGroupInviteConsumerMock.Expect(mock => mock.AddNewUserToGamingGroup(Arg<string>.Is.Anything, Arg<Guid>.Is.Anything))
                                         .Return(expectedNewlyRegisteredUser);
            firstTimeUserAuthenticatorMock.Expect(mock => mock.CreateGamingGroupAndSendEmailConfirmation(
                Arg<ApplicationUser>.Is.Anything,
                Arg<TransactionSource>.Is.Anything))
                .Return(Task.FromResult(expectedNewlyRegisteredUser));
        }

        [Test]
        public async Task ItCreatesANewUser()
        {
            await userRegisterer.RegisterUser(newUser);

            applicationUserManagerMock.AssertWasCalled(mock => mock.CreateAsync(
                Arg<ApplicationUser>.Matches(appUser => appUser.UserName == newUser.UserName
                    && appUser.Email == newUser.EmailAddress
                    && appUser.EmailConfirmed),
                Arg<string>.Is.Equal(newUser.Password)));
        }

        [Test]
        public async Task ItSignsInTheNewUserIfRegisteringFromTheWebApplication()
        {
            await userRegisterer.RegisterUser(newUser);

            signInManagerMock.AssertWasCalled(mock => mock.SignInAsync(
                Arg<ApplicationUser>.Matches(user => user.Email == newUser.EmailAddress && user.UserName == newUser.UserName), 
                Arg<bool>.Is.Equal(false),
                Arg<bool>.Is.Equal(false)));
        }

        [Test]
        public async Task ItDoesntSignInIfRegisteringFromTheReTaskstApi()
        {
            newUser.Source = TransactionSource.RestApi;

            await userRegisterer.RegisterUser(newUser);

            signInManagerMock.AssertWasNotCalled(mock => mock.SignInAsync(
                Arg<ApplicationUser>.Is.Anything,
                Arg<bool>.Is.Anything,
                Arg<bool>.Is.Anything));
        }

        [Test(Description = "added zzz prefix as this test needs to run last due to event tracking timing")]
        public async Task zzz_ItRecordsAUserRegisteredEvent()
        {
            await userRegisterer.RegisterUser(newUser);

            eventTrackerMock.AssertWasCalled(mock => mock.TrackUserRegistration(newUser.Source));
        }

        [Test]
        public async Task ItCreatesANewGamingGroupIfNotConsumingAnInvitationToAnExistingGamingGroup()
        {
            newUser = new NewUser()
            {
                UserName = "user name",
                EmailAddress = "the email",
                GamingGroupInvitationId = null
            };

            await userRegisterer.RegisterUser(newUser);

            firstTimeUserAuthenticatorMock.AssertWasCalled(mock => mock.CreateGamingGroupAndSendEmailConfirmation(
                Arg<ApplicationUser>.Matches(user => user.UserName == newUser.UserName
                                                && user.Email == newUser.EmailAddress),
                Arg<TransactionSource>.Is.Anything));
        }

        [Test]
        public async Task ItDoesNotCreateANewGamingGroupIfUserIsBeingLinkedToAnExistingOne()
        {
            await userRegisterer.RegisterUser(newUser);

            firstTimeUserAuthenticatorMock.AssertWasNotCalled(mock => mock.CreateGamingGroupAndSendEmailConfirmation(
                Arg<ApplicationUser>.Is.Anything,
                Arg<TransactionSource>.Is.Anything));
        }

        [Test]
        public async Task ItDoesntSignInIfTheUserIsntCreatedSuccessfully()
        {
            newUser = new NewUser();
            IdentityResult result = new IdentityResult("an error");

            applicationUserManagerMock = MockRepository.GenerateMock<ApplicationUserManager>(userStoreMock, dataProtectionProviderMock);
            applicationUserManagerMock.Expect(mock => mock.CreateAsync(Arg<ApplicationUser>.Is.Anything, Arg<string>.Is.Anything))
                .Return(Task.FromResult<IdentityResult>(result));
            userRegisterer = new UserRegisterer(
                applicationUserManagerMock, 
                firstTimeUserAuthenticatorMock, 
                dataContextMock, 
                signInManagerMock,
                eventTrackerMock,
                gamingGroupInviteConsumerMock);

            await userRegisterer.RegisterUser(newUser);

            firstTimeUserAuthenticatorMock.AssertWasNotCalled(mock => mock.CreateGamingGroupAndSendEmailConfirmation(
                Arg<ApplicationUser>.Is.Anything,
                Arg<TransactionSource>.Is.Anything));
        }

        [Test]
        public async Task ItAddsTheUserToAnExistingGamingGroupIfThereIsAnInvitation()
        {
            await userRegisterer.RegisterUser(newUser);

            gamingGroupInviteConsumerMock.AssertWasCalled(mock => mock.AddNewUserToGamingGroup(
                Arg<string>.Is.Equal(applicationUserIdAfterSaving), 
                    Arg<Guid>.Is.Equal(newUser.GamingGroupInvitationId.Value)));
        }

        [Test]
        public async Task ItReturnsTheNewlyRegisteredUser()
        {
            RegisterNewUserResult result = await userRegisterer.RegisterUser(newUser);

            Assert.That(result.NewlyRegisteredUser.UserId, Is.EqualTo(applicationUserIdAfterSaving));
            Assert.That(result.NewlyRegisteredUser.PlayerId, Is.EqualTo(expectedNewlyRegisteredUser.PlayerId));
            Assert.That(result.NewlyRegisteredUser.PlayerName, Is.EqualTo(expectedNewlyRegisteredUser.PlayerName));
            Assert.That(result.NewlyRegisteredUser.GamingGroupId, Is.EqualTo(expectedNewlyRegisteredUser.GamingGroupId));
            Assert.That(result.NewlyRegisteredUser.GamingGroupName, Is.EqualTo(expectedNewlyRegisteredUser.GamingGroupName));
        }
    }
}
