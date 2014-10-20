using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;
using UI.Controllers;
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
        protected IFirstTimeAuthenticator firstTimeAuthenticator;
        protected AccountController accountControllerPartialMock;
        protected RegisterViewModel registerViewModel;
        protected ApplicationUser newApplicationUser;

        [SetUp]
        public virtual void SetUp()
        {
            userStoreMock = MockRepository.GenerateMock<IUserStore<ApplicationUser>>();
            gamingGroupInviteConsumerMock = MockRepository.GenerateMock<IGamingGroupInviteConsumer>();
            userRegistererMock = MockRepository.GenerateMock<IUserRegisterer>();
            firstTimeAuthenticator = MockRepository.GenerateMock<IFirstTimeAuthenticator>();
            userManager = new ApplicationUserManager(userStoreMock);
            accountControllerPartialMock = MockRepository.GeneratePartialMock<AccountController>(
                userManager,
                userRegistererMock,
                firstTimeAuthenticator);
            newApplicationUser = new ApplicationUser()
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
