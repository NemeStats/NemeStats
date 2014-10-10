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
    public class RegisterHttpPostTests : AccountControllerTestBase
    {
        [Test]
        public async Task ItStaysOnTheRegisterPageIfThereAreErrors()
        {
            accountControllerPartialMock.ModelState.AddModelError("key", "an error message");

            ViewResult result = await accountControllerPartialMock.Register(new RegisterViewModel()) as ViewResult;

            Assert.AreEqual(MVC.Account.Views.Register, result.ViewName);
        }

        [Test]
        public async Task ItReloadsTheCurrentRegisterViewModelIfThereAreErrors()
        {
            accountControllerPartialMock.ModelState.AddModelError("key", "an error message");
            RegisterViewModel expectedViewModel = new RegisterViewModel();

            ViewResult result = await accountControllerPartialMock.Register(expectedViewModel) as ViewResult;

            Assert.AreSame(expectedViewModel, result.Model);
        }

        [Test]
        public async Task ItRegistersANewUserIfThereAreNoModelErrors()
        {
            RegisterViewModel expectedViewModel = new RegisterViewModel()
            {
                EmailAddress = "email",
                Password = "password",
                UserName = "user name"
            };

            ViewResult result = await accountControllerPartialMock.Register(expectedViewModel) as ViewResult;

            userRegistererMock.AssertWasCalled(mock => mock.RegisterUser(Arg<NewUser>.Matches(
                user => user.Email == expectedViewModel.EmailAddress
                    && user.UserName == expectedViewModel.UserName
                    && user.Password == expectedViewModel.Password)));
        }
    }
}
