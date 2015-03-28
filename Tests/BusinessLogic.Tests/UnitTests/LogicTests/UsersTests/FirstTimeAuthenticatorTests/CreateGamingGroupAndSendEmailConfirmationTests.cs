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
using System.Configuration;
using System.Configuration.Abstractions;
using System.Linq;
using System.Runtime.Remoting;
using System.Threading.Tasks;
using System.Web;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.DataProtection;
using NUnit.Framework;
using Rhino.Mocks;
using BusinessLogic.Models.GamingGroups;

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
        private NewlyCreatedGamingGroupResult expectedNewlyCreatedGamingGroupResult;
        protected IDataProtectionProvider dataProtectionProviderMock;
        private ApplicationUser applicationUser;
        private string confirmationToken = "the confirmation token";
        private string callbackUrl = "nemestats.com/Account/ConfirmEmail";
        private RegistrationSource registrationSource;

        [SetUp]
        public void SetUp()
        {
            var userStoreMock = MockRepository.GenerateMock<IUserStore<ApplicationUser>>();
            var dataProtector = MockRepository.GenerateMock<IDataProtector>();
            dataProtectionProviderMock = MockRepository.GenerateMock<IDataProtectionProvider>();
            dataProtectionProviderMock.Expect(mock => mock.Create(Arg<string>.Is.Anything)).Return(dataProtector);
            applicationUserManagerMock = MockRepository.GenerateMock<ApplicationUserManager>(userStoreMock, dataProtectionProviderMock);
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

            registrationSource = RegistrationSource.RestApi;

            var appSettingsMock = MockRepository.GenerateMock<IAppSettings>();
            configurationManagerMock.Expect(mock => mock.AppSettings)
                                    .Return(appSettingsMock);
            appSettingsMock.Expect(mock => mock.Get(FirstTimeAuthenticator.APP_KEY_EMAIL_CONFIRMATION_CALLBACK_URL))
                           .Return(callbackUrl);

            expectedNewlyCreatedGamingGroupResult = new NewlyCreatedGamingGroupResult
            {
                NewlyCreatedGamingGroup = new GamingGroup {  Id = 1 },
                NewlyCreatedPlayer = new Player { Id = 100 }
            };
            gamingGroupSaverMock.Expect(mock => mock.CreateNewGamingGroup(
                                                                          Arg<string>.Is.Anything,
                                                                          Arg<RegistrationSource>.Is.Anything,
                                                                          Arg<ApplicationUser>.Is.Anything))
                                .Return(expectedNewlyCreatedGamingGroupResult);

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
            await firstTimeAuthenticator.CreateGamingGroupAndSendEmailConfirmation(applicationUser, registrationSource);

            gamingGroupSaverMock.AssertWasCalled(mock => mock.CreateNewGamingGroup(
                                                                                   Arg<string>.Is.Equal(applicationUser.UserName + "'s Gaming Group"),
                                                                                   Arg<RegistrationSource>.Is.Equal(registrationSource),
                                                                                   Arg<ApplicationUser>.Is.Same(applicationUser)));
        }

        [Test]
        public async Task ItEmailsNewRegistrantsAskingForConfirmation()
        {
            await firstTimeAuthenticator.CreateGamingGroupAndSendEmailConfirmation(applicationUser, registrationSource);

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
                await firstTimeAuthenticator.CreateGamingGroupAndSendEmailConfirmation(applicationUser, registrationSource);
            }
            catch (ConfigurationException expectedException)
            {
                exceptionMessage = expectedException.Message;
            }

            Assert.AreEqual(exceptionMessage, "Missing app setting with key: emailConfirmationCallbackUrl");
        }

        [Test]
        public async Task ItReturnsTheNewlyCreatedGamingGroupResult()
        {
            NewlyCreatedGamingGroupResult result = await firstTimeAuthenticator.CreateGamingGroupAndSendEmailConfirmation(applicationUser, registrationSource);

            Assert.That(result, Is.SameAs(expectedNewlyCreatedGamingGroupResult));
        }
    }
}