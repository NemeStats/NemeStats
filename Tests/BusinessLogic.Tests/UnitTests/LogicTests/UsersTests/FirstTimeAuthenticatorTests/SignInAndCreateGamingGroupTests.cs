using System.Net.Mime;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.UsersTests.FirstTimeAuthenticatorTests
{
    [TestFixture]
    public class SignInAndCreateGamingGroupTests
    {
        private IAuthenticationManager authenticationManagerMock;
        private INemeStatsEventTracker eventTrackerMock;
        private ApplicationSignInManager signInManagerMock;
        private IGamingGroupInviteConsumer gamingGroupInviteConsumerMock;
        private IGamingGroupSaver gamingGroupSaverMock;
        private FirstTimeAuthenticator firstTimeAuthenticator;
        private ApplicationUser applicationUser;

        [SetUp]
        public void SetUp()
        {
            authenticationManagerMock = MockRepository.GenerateMock<IAuthenticationManager>();
            eventTrackerMock = MockRepository.GenerateMock<INemeStatsEventTracker>();
            IUserStore<ApplicationUser> userStoreMock = MockRepository.GenerateMock<IUserStore<ApplicationUser>>();
            ApplicationUserManager applicationUserManagerMock = MockRepository.GenerateMock<ApplicationUserManager>(userStoreMock);
            signInManagerMock = MockRepository.GenerateMock<ApplicationSignInManager>(applicationUserManagerMock, authenticationManagerMock);
            gamingGroupInviteConsumerMock = MockRepository.GenerateMock<IGamingGroupInviteConsumer>();
            gamingGroupSaverMock = MockRepository.GenerateMock<IGamingGroupSaver>();

            firstTimeAuthenticator = new FirstTimeAuthenticator(
                authenticationManagerMock,
                eventTrackerMock,
                signInManagerMock,
                gamingGroupInviteConsumerMock,
                gamingGroupSaverMock);

            applicationUser = new ApplicationUser()
            {
                UserName = "user name"
            };

            eventTrackerMock.Expect(mock => mock.TrackUserRegistration());
            signInManagerMock.Expect(mock => mock.SignInAsync(
                                                              Arg<ApplicationUser>.Is.Anything,
                                                              Arg<bool>.Is.Anything,
                                                              Arg<bool>.Is.Anything))
                             .Return(Task.FromResult(-1));

            gamingGroupSaverMock.Expect(mock => mock.CreateNewGamingGroup(
                                                                Arg<string>.Is.Anything,
                                                                Arg<ApplicationUser>.Is.Anything))
                      .Return(Task.FromResult(new GamingGroup()));
        }

        [Test]
        public async Task ItTracksTheUserRegistration()
        {
            gamingGroupInviteConsumerMock.Expect(mock => mock.ConsumeGamingGroupInvitation(Arg<ApplicationUser>.Is.Anything))
                                         .Return(Task.FromResult((int?)null));

            await firstTimeAuthenticator.SignInAndCreateGamingGroup(applicationUser);

            eventTrackerMock.AssertWasCalled(mock => mock.TrackUserRegistration());
        }

        [Test]
        public async Task ItSignsInTheUser()
        {
            gamingGroupInviteConsumerMock.Expect(mock => mock.ConsumeGamingGroupInvitation(Arg<ApplicationUser>.Is.Anything))
                                         .Return(Task.FromResult((int?)null));

            await firstTimeAuthenticator.SignInAndCreateGamingGroup(applicationUser);

            signInManagerMock.AssertWasCalled(mock => mock.SignInAsync(applicationUser, false, false));
        }

        [Test]
        public async Task ItConsumesAnyGamingGroupInvitations()
        {
            gamingGroupInviteConsumerMock.Expect(mock => mock.ConsumeGamingGroupInvitation(Arg<ApplicationUser>.Is.Anything))
                                         .Return(Task.FromResult((int?)null));

            await firstTimeAuthenticator.SignInAndCreateGamingGroup(applicationUser);

            gamingGroupInviteConsumerMock.AssertWasCalled(mock => mock.ConsumeGamingGroupInvitation(applicationUser));
        }

        [Test]
        public async Task ItCreatesANewGamingGroupForTheUserIfTheyWerentAddedToAnExistingOne()
        {
            gamingGroupInviteConsumerMock.Expect(mock => mock.ConsumeGamingGroupInvitation(Arg<ApplicationUser>.Is.Anything))
                                         .Return(Task.FromResult((int?)null));
  
            await firstTimeAuthenticator.SignInAndCreateGamingGroup(applicationUser);

            gamingGroupSaverMock.AssertWasCalled(mock => mock.CreateNewGamingGroup(
                Arg<string>.Is.Equal(applicationUser.UserName + "'s Gaming Group"),
                Arg<ApplicationUser>.Is.Same(applicationUser)));
        }

        [Test]
        public async Task ItDoesNotCreateANewGamingGroupForTheUserIfTheyWereAddedToAnExistingOne()
        {
            int? existingGamingGroupId = 1;
            gamingGroupInviteConsumerMock.Expect(mock => mock.ConsumeGamingGroupInvitation(Arg<ApplicationUser>.Is.Anything))
                                         .Return(Task.FromResult(existingGamingGroupId));

            await firstTimeAuthenticator.SignInAndCreateGamingGroup(applicationUser);

            gamingGroupSaverMock.AssertWasNotCalled(mock => mock.CreateNewGamingGroup(
                Arg<string>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
        }
    }
}
