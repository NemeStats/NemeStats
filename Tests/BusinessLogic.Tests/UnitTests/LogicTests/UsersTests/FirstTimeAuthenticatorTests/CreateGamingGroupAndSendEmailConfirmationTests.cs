using System;
using System.Configuration;
using System.Configuration.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.UsersTests.FirstTimeAuthenticatorTests
{
    [TestFixture]
    public class CreateGamingGroupAndSendEmailConfirmationTests
    {
        private IGamingGroupSaver gamingGroupSaverMock;
        private IConfigurationManager configurationManagerMock;
        private ApplicationUserManager applicationUserManagerMock;
        private IDataContext dataContextMock;
        private FirstTimeAuthenticator firstTimeAuthenticator;
        private ApplicationUser applicationUser;
        private string confirmationToken = "the confirmation token";
        private string callbackUrl = "nemestats.com/Account/ConfirmEmail";

        [SetUp]
        public void SetUp()
        {
            var userStoreMock = MockRepository.GenerateMock<IUserStore<ApplicationUser>>();
            applicationUserManagerMock = MockRepository.GenerateMock<ApplicationUserManager>(userStoreMock);
            gamingGroupSaverMock = MockRepository.GenerateMock<IGamingGroupSaver>();
            configurationManagerMock = MockRepository.GenerateMock<IConfigurationManager>();
            dataContextMock = MockRepository.GenerateMock<IDataContext>();

            firstTimeAuthenticator = new FirstTimeAuthenticator(
                gamingGroupSaverMock,
                applicationUserManagerMock,
                configurationManagerMock,
                dataContextMock);

            applicationUser = new ApplicationUser
            {
                Id = "user id",
                UserName = "user name"
            };

            var appSettingsMock = MockRepository.GenerateMock<IAppSettings>();
            configurationManagerMock.Expect(mock => mock.AppSettings)
                                    .Return(appSettingsMock);
            appSettingsMock.Expect(mock => mock.Get(FirstTimeAuthenticator.APP_KEY_EMAIL_CONFIRMATION_CALLBACK_URL))
                           .Return(callbackUrl);

            gamingGroupSaverMock.Expect(mock => mock.CreateNewGamingGroup(
                                                                          Arg<string>.Is.Anything,
                                                                          Arg<ApplicationUser>.Is.Anything))
                                .Return(Task.FromResult(new GamingGroup()));

            applicationUserManagerMock.Expect(mock => mock.GenerateEmailConfirmationTokenAsync(applicationUser.Id))
                                      .Return(Task.FromResult(confirmationToken));

            string expectedCallbackUrl = callbackUrl + string.Format(
                                                                     FirstTimeAuthenticator.CONFIRMATION_EMAIL_CALLBACK_URL_SUFFIX,
                                                                     applicationUser.Id,
                                                                     HttpUtility.UrlEncode(confirmationToken));
            string expectedEmailBody = string.Format(FirstTimeAuthenticator.CONFIRMATION_EMAIL_BODY, expectedCallbackUrl);
            applicationUserManagerMock.Expect(mock => mock.SendEmailAsync(
                                                                          applicationUser.Id,
                                                                          FirstTimeAuthenticator.EMAIL_SUBJECT,
                                                                          expectedEmailBody))
                                      .Return(Task.FromResult(-1));
        }

        [Test]
        public async Task ItCreatesANewGamingGroupForTheUser()
        {
            await firstTimeAuthenticator.CreateGamingGroupAndSendEmailConfirmation(applicationUser);

            gamingGroupSaverMock.AssertWasCalled(mock => mock.CreateNewGamingGroup(
                                                                                   Arg<string>.Is.Equal(applicationUser.UserName + "'s Gaming Group"),
                                                                                   Arg<ApplicationUser>.Is.Same(applicationUser)));
        }

        [Test]
        public async Task ItEmailsNewRegistrantsAskingForConfirmation()
        {
            await firstTimeAuthenticator.CreateGamingGroupAndSendEmailConfirmation(applicationUser);

            applicationUserManagerMock.VerifyAllExpectations();
        }

        [Test]
        public async Task ItThrowsAConfigurationExceptionIfTheCallbackUrlConfigSettingIsMissing()
        {
            configurationManagerMock = MockRepository.GenerateMock<IConfigurationManager>();
            var appSettingsMock = MockRepository.GenerateMock<IAppSettings>();
            configurationManagerMock.Expect(mock => mock.AppSettings)
                                    .Return(appSettingsMock);
            appSettingsMock.Expect(mock => mock.Get(FirstTimeAuthenticator.APP_KEY_EMAIL_CONFIRMATION_CALLBACK_URL))
                           .Throw(new Exception());

            firstTimeAuthenticator = new FirstTimeAuthenticator(
                gamingGroupSaverMock,
                applicationUserManagerMock,
                configurationManagerMock,
                dataContextMock);

            string exceptionMessage = string.Empty;
            try
            {
                await firstTimeAuthenticator.CreateGamingGroupAndSendEmailConfirmation(applicationUser);
            }
            catch (ConfigurationException expectedException)
            {
                exceptionMessage = expectedException.Message;
            }

            Assert.AreEqual(exceptionMessage, "Missing app setting with key: emailConfirmationCallbackUrl");
        }
    }
}