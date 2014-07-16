using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using UI.Controllers;
using UI.Models;

namespace UI.Tests.UnitTests.ControllerTests.AccountControllerTests
{
    [TestFixture]
    public class AccountControllerTestBase
    {
        protected UserManager<ApplicationUser> userManager;
        protected IUserStore<ApplicationUser> userStoreMock;
        protected AccountController accountControllerPartialMock;
        //protected ModelStateDictionary modelStateDictionaryMock;
        protected RegisterViewModel registerViewModel;

        [SetUp]
        public void SetUp()
        {
            userStoreMock = MockRepository.GenerateMock<IUserStore<ApplicationUser>>();
            userManager = new UserManager<ApplicationUser>(userStoreMock);
            accountControllerPartialMock = MockRepository.GeneratePartialMock<AccountController>(userManager);
           // modelStateDictionaryMock = MockRepository.GenerateMock<ModelStateDictionary>();
            
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
