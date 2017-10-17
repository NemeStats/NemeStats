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

using BoardGameGeekApiClient.Interfaces;
using BusinessLogic.Logic;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Controllers;
using UI.Models;

namespace UI.Tests.UnitTests.ControllerTests.AccountControllerTests
{
    public class AccountControllerTestBase
    {
        protected ApplicationUserManager userManager;
        protected IUserStore<ApplicationUser> userStoreMock;
        protected IGamingGroupInviteConsumer gamingGroupInviteConsumerMock;
        protected IUserRegisterer userRegistererMock;
        protected IFirstTimeAuthenticator firstTimeAuthenticatorMock;
        protected IAuthenticationManager authenticationManagerMock;
        protected AccountController accountControllerPartialMock;
        protected IDataProtectionProvider dataProtectionProviderMock;
        protected IGamingGroupRetriever gamingGroupRetrieverMock;
        protected ITransformer transformerMock;
        protected IGamingGroupContextSwitcher gamingGroupContextSwitcher;
        protected RegisterViewModel registerViewModel;
        protected ApplicationUser currentUser;
        IBoardGameGeekApiClient boardGameGeekApiClient;
        IBoardGameGeekUserSaver boardGameGeekUserSaver;
        private IUserRetriever userRetriever;

        [SetUp]
        public virtual void SetUp()
        {
            userStoreMock = MockRepository.GenerateMock<IUserStore<ApplicationUser>>();
            gamingGroupInviteConsumerMock = MockRepository.GenerateMock<IGamingGroupInviteConsumer>();
            userRegistererMock = MockRepository.GenerateMock<IUserRegisterer>();
            firstTimeAuthenticatorMock = MockRepository.GenerateMock<IFirstTimeAuthenticator>();
            authenticationManagerMock = MockRepository.GenerateMock<IAuthenticationManager>();
            var dataProtector = MockRepository.GenerateMock<IDataProtector>();
            dataProtectionProviderMock = MockRepository.GenerateMock<IDataProtectionProvider>();
            gamingGroupRetrieverMock = MockRepository.GenerateMock<IGamingGroupRetriever>();
            
            boardGameGeekUserSaver = MockRepository.GenerateMock<IBoardGameGeekUserSaver>();
            boardGameGeekApiClient = MockRepository.GenerateMock<IBoardGameGeekApiClient>();
            userRetriever = MockRepository.GenerateMock<IUserRetriever>();
            transformerMock = MockRepository.GenerateMock<ITransformer>();
            gamingGroupContextSwitcher = MockRepository.GenerateMock<IGamingGroupContextSwitcher>();

            dataProtectionProviderMock.Expect(mock => mock.Create(Arg<string>.Is.Anything)).Return(dataProtector);

            userManager = new ApplicationUserManager(userStoreMock, dataProtectionProviderMock);
            accountControllerPartialMock = MockRepository.GeneratePartialMock<AccountController>(
                userManager,
                userRegistererMock,
                firstTimeAuthenticatorMock,
                authenticationManagerMock,
                gamingGroupInviteConsumerMock, 
                gamingGroupRetrieverMock,
                boardGameGeekUserSaver,
                boardGameGeekApiClient,
                userRetriever,
                transformerMock,
                gamingGroupContextSwitcher);
            currentUser = new ApplicationUser()
            {
                Id = "new application user",
                CurrentGamingGroupId = 1
            };
            registerViewModel = new RegisterViewModel()
            {
                ConfirmPassword = "confirm password",
                Password = "password",
                UserName = "user name",
                EmailAddress = "email@email.com"
            };
        }
    }
}
