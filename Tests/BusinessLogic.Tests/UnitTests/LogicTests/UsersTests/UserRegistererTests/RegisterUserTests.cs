using System;
using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
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
        private INemeStatsEventTracker eventTrackerMock;
        private IGamingGroupInviteConsumer gamingGroupInviteConsumerMock;

        private NewUser newUser;
        private string applicationUserIdAfterSaving = "new application user Id";

        [SetUp]
        public void SetUp()
        {
            firstTimeUserAuthenticatorMock = MockRepository.GenerateMock<IFirstTimeAuthenticator>();
            userStoreMock = MockRepository.GenerateMock<IUserStore<ApplicationUser>>();
            applicationUserManagerMock = MockRepository.GenerateMock<ApplicationUserManager>(userStoreMock);
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
            int playerId = 1938;
            GamingGroupInvitation invitation = new GamingGroupInvitation
            {
                Id = invitationId,
                PlayerId = playerId
            };
            newUser = new NewUser()
            {
                UserName = "user name",
                Email = "the email",
                GamingGroupInvitationId = invitationId
            };
            IdentityResult result = IdentityResult.Success;

            dataContextMock.Expect(mock => mock.FindById<GamingGroupInvitation>(invitationId))
                           .Return(invitation);
            applicationUserManagerMock.Expect(mock => mock.CreateAsync(Arg<ApplicationUser>.Is.Anything, Arg<string>.Is.Anything))
                .Return(Task.FromResult(result));
            signInManagerMock.Expect(mock => mock.SignInAsync(
                                                                          Arg<ApplicationUser>.Is.Anything,
                                                                          Arg<bool>.Is.Anything,
                                                                          Arg<bool>.Is.Anything))
                                         .WhenCalled(invocation => ((ApplicationUser)invocation.Arguments[0]).Id = this.applicationUserIdAfterSaving)
                                         .Return(Task.FromResult(-1));
            firstTimeUserAuthenticatorMock.Expect(mock => mock.CreateGamingGroup(Arg<ApplicationUser>.Is.Anything))
                .Return(Task.FromResult(new object()));
        }

        [Test]
        public async Task ItCreatesANewUser()
        {
            await userRegisterer.RegisterUser(newUser);

            applicationUserManagerMock.AssertWasCalled(mock => mock.CreateAsync(
                Arg<ApplicationUser>.Matches(appUser => appUser.UserName == newUser.UserName
                    && appUser.Email == newUser.Email
                    && appUser.EmailConfirmed),
                Arg<string>.Is.Equal(newUser.Password)));
        }

        [Test]
        public async Task ItSignsInTheNewUser()
        {
            await userRegisterer.RegisterUser(newUser);

            signInManagerMock.AssertWasCalled(mock => mock.SignInAsync(
                Arg<ApplicationUser>.Matches(user => user.Email == newUser.Email && user.UserName == newUser.UserName), 
                Arg<bool>.Is.Equal(false),
                Arg<bool>.Is.Equal(false)));
        }

        [Test]
        public async Task ItRecordsAUserRegisteredEvent()
        {
            await userRegisterer.RegisterUser(newUser);

            eventTrackerMock.AssertWasCalled(mock => mock.TrackUserRegistration());
        }

        [Test]
        public async Task ItCreatesANewGamingGroupIfNotConsumingAnInvitationToAnExistingGamingGroup()
        {
            NewUser newUser = new NewUser()
            {
                UserName = "user name",
                Email = "the email",
                GamingGroupInvitationId = null
            };

            await userRegisterer.RegisterUser(newUser);

            firstTimeUserAuthenticatorMock.AssertWasCalled(mock => mock.CreateGamingGroup(
                Arg<ApplicationUser>.Matches(user => user.UserName == newUser.UserName
                                                && user.Email == newUser.Email)));
        }

        [Test]
        public async Task ItDoesNotCreateANewGamingGroupIfUserIsBeingLinkedToAnExistingOne()
        {
            await userRegisterer.RegisterUser(newUser);

            firstTimeUserAuthenticatorMock.AssertWasNotCalled(mock => mock.CreateGamingGroup(
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public async Task ItDoesntSignInIfTheUserIsntCreatedSuccessfully()
        {
            NewUser newUser = new NewUser();
            IdentityResult result = new IdentityResult("an error");
            applicationUserManagerMock = MockRepository.GenerateMock<ApplicationUserManager>(userStoreMock);
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

            firstTimeUserAuthenticatorMock.AssertWasNotCalled(mock => mock.CreateGamingGroup(
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public async Task ItAddsTheUserToAnExistingGamingGroupIfThereIsAnInvitation()
        {
            await userRegisterer.RegisterUser(newUser);

            gamingGroupInviteConsumerMock.AssertWasCalled(mock => mock.AddNewUserToGamingGroup(
                Arg<string>.Is.Equal(applicationUserIdAfterSaving), 
                    Arg<Guid>.Is.Equal(newUser.GamingGroupInvitationId.Value)));
        }
    }
}
