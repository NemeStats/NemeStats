using System.Configuration;
using System.Configuration.Abstractions;
using System.Web;
using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Linq;
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
        private IConfigurationManager configurationManagerMock;
        private ApplicationUserManager applicationUserManagerMock;
        private IDataContext dataContextMock;
        private FirstTimeAuthenticator firstTimeAuthenticator;
        private ApplicationUser applicationUser;
        private string applicationUserIdAfterSaving = "application user id after saving";
        private string confirmationToken = "the confirmation token";
        private string callbackUrl = "nemestats.com/Account/ConfirmEmail";

        [SetUp]
        public void SetUp()
        {
            authenticationManagerMock = MockRepository.GenerateMock<IAuthenticationManager>();
            eventTrackerMock = MockRepository.GenerateMock<INemeStatsEventTracker>();
            IUserStore<ApplicationUser> userStoreMock = MockRepository.GenerateMock<IUserStore<ApplicationUser>>();
            applicationUserManagerMock = MockRepository.GenerateMock<ApplicationUserManager>(userStoreMock);
            signInManagerMock = MockRepository.GenerateMock<ApplicationSignInManager>(applicationUserManagerMock, authenticationManagerMock);
            gamingGroupInviteConsumerMock = MockRepository.GenerateMock<IGamingGroupInviteConsumer>();
            gamingGroupSaverMock = MockRepository.GenerateMock<IGamingGroupSaver>();
            configurationManagerMock = MockRepository.GenerateMock<IConfigurationManager>();
            dataContextMock = MockRepository.GenerateMock<IDataContext>();

            firstTimeAuthenticator = new FirstTimeAuthenticator(
                eventTrackerMock,
                signInManagerMock,
                gamingGroupInviteConsumerMock,
                gamingGroupSaverMock,
                applicationUserManagerMock,
                configurationManagerMock,
                dataContextMock);

            applicationUser = new ApplicationUser()
            {
                Id = "user id",
                UserName = "user name"
            };

            IAppSettings appSettingsMock = MockRepository.GenerateMock<IAppSettings>();
            configurationManagerMock.Expect(mock => mock.AppSettings)
                                    .Return(appSettingsMock);
            appSettingsMock.Expect(mock => mock.Get(FirstTimeAuthenticator.APP_KEY_EMAIL_CONFIRMATION_CALLBACK_URL))
                           .Return(callbackUrl);
            eventTrackerMock.Expect(mock => mock.TrackUserRegistration());
            signInManagerMock.Expect(mock => mock.SignInAsync(
                                                              Arg<ApplicationUser>.Is.Anything,
                                                              Arg<bool>.Is.Anything,
                                                              Arg<bool>.Is.Anything))
                             .WhenCalled(invocation => this.applicationUser.Id = this.applicationUserIdAfterSaving)
                             .Return(Task.FromResult(-1));

            gamingGroupSaverMock.Expect(mock => mock.CreateNewGamingGroup(
                                                                Arg<string>.Is.Anything,
                                                                Arg<ApplicationUser>.Is.Anything))
                      .Return(Task.FromResult(new GamingGroup()));

            applicationUserManagerMock.Expect(mock => mock.GenerateEmailConfirmationTokenAsync(this.applicationUserIdAfterSaving))
                                      .Return(Task.FromResult(confirmationToken));

            string expectedCallbackUrl = callbackUrl + string.Format(
                FirstTimeAuthenticator.CONFIRMATION_EMAIL_CALLBACK_URL_SUFFIX,
                this.applicationUserIdAfterSaving, 
                HttpUtility.UrlEncode(confirmationToken));
            string expectedEmailBody = string.Format(FirstTimeAuthenticator.CONFIRMATION_EMAIL_BODY, expectedCallbackUrl);
            applicationUserManagerMock.Expect(mock => mock.SendEmailAsync(
                                                                          this.applicationUserIdAfterSaving,
                                                                          FirstTimeAuthenticator.EMAIL_SUBJECT,
                                                                          expectedEmailBody))
                                      .Return(Task.FromResult(-1));
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

        [Test]
        public async Task ItEmailsNewRegistrantsAskingForConfirmation()
        {
            gamingGroupInviteConsumerMock.Expect(mock => mock.ConsumeGamingGroupInvitation(Arg<ApplicationUser>.Is.Anything))
                             .Return(Task.FromResult((int?)null));

            await firstTimeAuthenticator.SignInAndCreateGamingGroup(applicationUser);

            applicationUserManagerMock.VerifyAllExpectations();
        }

        [Test]
        public async Task ItThrowsAConfigurationExceptionIfTheCallbackUrlConfigSettingIsMissing()
        {
            configurationManagerMock = MockRepository.GenerateMock<IConfigurationManager>();
            IAppSettings appSettingsMock = MockRepository.GenerateMock<IAppSettings>();
            configurationManagerMock.Expect(mock => mock.AppSettings)
                                    .Return(appSettingsMock);
            appSettingsMock.Expect(mock => mock.Get(FirstTimeAuthenticator.APP_KEY_EMAIL_CONFIRMATION_CALLBACK_URL))
                           .Throw(new Exception());

            firstTimeAuthenticator = new FirstTimeAuthenticator(
                eventTrackerMock,
                signInManagerMock,
                gamingGroupInviteConsumerMock,
                gamingGroupSaverMock,
                applicationUserManagerMock,
                configurationManagerMock,
                dataContextMock);

            string exceptionMessage = string.Empty;
            try
            {
                await firstTimeAuthenticator.SignInAndCreateGamingGroup(applicationUser);
            }
            catch (ConfigurationException expectedException)
            {
                exceptionMessage = expectedException.Message;
            }

            Assert.AreEqual(exceptionMessage, "Missing app setting with key: emailConfirmationCallbackUrl");
        }

        [Test]
        public async Task ItCreatesAUserGamingGroupRecord()
        {
            gamingGroupInviteConsumerMock.Expect(mock => mock.ConsumeGamingGroupInvitation(Arg<ApplicationUser>.Is.Anything))
                 .Return(Task.FromResult((int?)null));

            await firstTimeAuthenticator.SignInAndCreateGamingGroup(applicationUser);

            dataContextMock.AssertWasCalled(mock => mock.Save(Arg<UserGamingGroup>.Matches(userGamingGroup => userGamingGroup.ApplicationUserId == applicationUserIdAfterSaving),
                Arg<ApplicationUser>.Is.Anything));
        }
    }
}
