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
