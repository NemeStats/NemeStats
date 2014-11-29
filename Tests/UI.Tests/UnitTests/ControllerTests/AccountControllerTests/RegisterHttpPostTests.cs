using System;
using System.Web;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using UI.Controllers.Helpers;
using UI.Models;

namespace UI.Tests.UnitTests.ControllerTests.AccountControllerTests
{
    [TestFixture]
    public class RegisterHttpPostTests : AccountControllerTestBase
    {
        private RegisterViewModel expectedViewModel;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            expectedViewModel = new RegisterViewModel()
            {
                EmailAddress = "email",
                Password = "password",
                UserName = "user name",
                GamingGroupInvitationId = Guid.NewGuid().ToString()
            };
        }

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

            ViewResult result = await accountControllerPartialMock.Register(expectedViewModel) as ViewResult;

            Assert.AreSame(expectedViewModel, result.Model);
        }

        [Test]
        public async Task ItRegistersANewUserIfThereAreNoModelErrors()
        {
            userRegistererMock.Expect(mock => mock.RegisterUser(Arg<NewUser>.Is.Anything))
                              .Return(Task.FromResult(IdentityResult.Success));

            ViewResult result = await accountControllerPartialMock.Register(expectedViewModel) as ViewResult;

            userRegistererMock.AssertWasCalled(mock => mock.RegisterUser(Arg<NewUser>.Matches(
                user => user.Email == expectedViewModel.EmailAddress
                    && user.UserName == expectedViewModel.UserName
                    && user.Password == expectedViewModel.Password
                    && user.GamingGroupInvitationId == new Guid(expectedViewModel.GamingGroupInvitationId))));
        }

        [Test]
        public async Task ItRedirectsToTheGamingGroupPageIfTheUserIsSuccessfullyRegistered()
        {
            userRegistererMock.Expect(mock => mock.RegisterUser(Arg<NewUser>.Is.Anything))
                              .Return(Task.FromResult(IdentityResult.Success));

            RedirectToRouteResult result = await accountControllerPartialMock.Register(expectedViewModel) as RedirectToRouteResult;

            Assert.AreEqual(MVC.GamingGroup.ActionNames.Index, result.RouteValues["action"]);
        }

        [Test]
        public async Task ItAddsErrorsAndStaysOnThePageIfRegisteringFails()
        {
            string errorMessage = "some error message";
            IdentityResult failureIdentityResult = new IdentityResult(new string[]
            {
                errorMessage
            });
            userRegistererMock.Expect(mock => mock.RegisterUser(Arg<NewUser>.Is.Anything))
                              .Return(Task.FromResult(failureIdentityResult));

            ViewResult result = await accountControllerPartialMock.Register(expectedViewModel) as ViewResult;

            Assert.AreSame(expectedViewModel, result.Model);
            Assert.AreEqual(MVC.Account.Views.Register, result.ViewName);
            Assert.True(accountControllerPartialMock.ModelState[string.Empty].Errors.Any(error => error.ErrorMessage == errorMessage));
        }

        [Test]
        public async Task ItClearsTheGamingGroupCookieIfTheUserSuccessfullyRegisters()
        {
            userRegistererMock.Expect(mock => mock.RegisterUser(Arg<NewUser>.Is.Anything))
                  .Return(Task.FromResult(IdentityResult.Success));

            await accountControllerPartialMock.Register(expectedViewModel);
            
            cookieHelperMock.AssertWasCalled(mock => mock.ClearCookie(
                Arg<NemeStatsCookieEnum>.Is.Equal(NemeStatsCookieEnum.gamingGroupsCookie),
                Arg<HttpRequestBase>.Is.Anything,
                Arg<HttpResponseBase>.Is.Anything));
        }
    }
}
