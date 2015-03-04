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
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;
using UI.Controllers;
using UI.Controllers.Helpers;
using UI.Models;

namespace UI.Tests.UnitTests.ControllerTests.AccountControllerTests
{
    [TestFixture]
    public class AccountControllerTestBase
    {
        protected ApplicationUserManager userManager;
        protected IUserStore<ApplicationUser> userStoreMock;
        protected IGamingGroupInviteConsumer gamingGroupInviteConsumerMock;
        protected IUserRegisterer userRegistererMock;
        protected IFirstTimeAuthenticator firstTimeAuthenticatorMock;
        protected IAuthenticationManager authenticationManagerMock;
        protected ICookieHelper cookieHelperMock;
        protected AccountController accountControllerPartialMock;
        protected RegisterViewModel registerViewModel;
        protected ApplicationUser currentUser;

        [SetUp]
        public virtual void SetUp()
        {
            userStoreMock = MockRepository.GenerateMock<IUserStore<ApplicationUser>>();
            gamingGroupInviteConsumerMock = MockRepository.GenerateMock<IGamingGroupInviteConsumer>();
            userRegistererMock = MockRepository.GenerateMock<IUserRegisterer>();
            this.firstTimeAuthenticatorMock = MockRepository.GenerateMock<IFirstTimeAuthenticator>();
            this.authenticationManagerMock = MockRepository.GenerateMock<IAuthenticationManager>();
            userManager = new ApplicationUserManager(userStoreMock);
            cookieHelperMock = MockRepository.GenerateMock<ICookieHelper>();
            accountControllerPartialMock = MockRepository.GeneratePartialMock<AccountController>(
                userManager,
                userRegistererMock,
                this.firstTimeAuthenticatorMock,
                this.authenticationManagerMock,
                gamingGroupInviteConsumerMock,
                cookieHelperMock);
            currentUser = new ApplicationUser()
            {
                Id = "new application user"
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
