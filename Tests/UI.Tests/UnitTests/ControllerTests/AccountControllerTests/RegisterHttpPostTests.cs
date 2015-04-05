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
            RegisterNewUserResult registerNewUserResult = new RegisterNewUserResult
            {
                Result = IdentityResult.Success
            };
            userRegistererMock.Expect(mock => mock.RegisterUser(Arg<NewUser>.Is.Anything))
                              .Return(Task.FromResult(registerNewUserResult));

            ViewResult result = await accountControllerPartialMock.Register(expectedViewModel) as ViewResult;

            userRegistererMock.AssertWasCalled(mock => mock.RegisterUser(Arg<NewUser>.Matches(
                user => user.EmailAddress == expectedViewModel.EmailAddress
                    && user.UserName == expectedViewModel.UserName
                    && user.Password == expectedViewModel.Password
                    && user.GamingGroupInvitationId == new Guid(expectedViewModel.GamingGroupInvitationId))));
        }

        [Test]
        public async Task ItRedirectsToTheGamingGroupPageIfTheUserIsSuccessfullyRegistered()
        {
            RegisterNewUserResult registerNewUserResult = new RegisterNewUserResult
            {
                Result = IdentityResult.Success
            };
            userRegistererMock.Expect(mock => mock.RegisterUser(Arg<NewUser>.Is.Anything))
                              .Return(Task.FromResult(registerNewUserResult));

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
            RegisterNewUserResult registerNewUserResult = new RegisterNewUserResult
            {
                Result = failureIdentityResult
            };
            userRegistererMock.Expect(mock => mock.RegisterUser(Arg<NewUser>.Is.Anything))
                              .Return(Task.FromResult(registerNewUserResult));

            ViewResult result = await accountControllerPartialMock.Register(expectedViewModel) as ViewResult;

            Assert.AreSame(expectedViewModel, result.Model);
            Assert.AreEqual(MVC.Account.Views.Register, result.ViewName);
            Assert.True(accountControllerPartialMock.ModelState[string.Empty].Errors.Any(error => error.ErrorMessage == errorMessage));
        }

        [Test]
        public async Task ItClearsTheGamingGroupCookieIfTheUserSuccessfullyRegisters()
        {
            RegisterNewUserResult registerNewUserResult = new RegisterNewUserResult
            {
                Result = IdentityResult.Success
            };
            userRegistererMock.Expect(mock => mock.RegisterUser(Arg<NewUser>.Is.Anything))
                  .Return(Task.FromResult(registerNewUserResult));

            await accountControllerPartialMock.Register(expectedViewModel);

            cookieHelperMock.AssertWasCalled(mock => mock.ClearCookie(
                Arg<NemeStatsCookieEnum>.Is.Equal(NemeStatsCookieEnum.gamingGroupsCookie),
                Arg<HttpRequestBase>.Is.Anything,
                Arg<HttpResponseBase>.Is.Anything));
        }
    }
}
