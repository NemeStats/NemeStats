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
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.DataProtection;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Attributes.Filters;
using UI.Controllers;
using UI.Models;

namespace UI.Tests.UnitTests.ControllerTests.AccountControllerTests
{
    [TestFixture]
    public class AccountControllerForbiddenAntiForgeryTests : AccountControllerTestBase
    {
        [TestCase(nameof(AccountController.Disassociate), new[] { typeof(string), typeof(string) })]
        [TestCase(nameof(AccountController.SetPassword), new[] { typeof(SetPasswordViewModel) })]
        [TestCase(nameof(AccountController.ChangePassword), new[] { typeof(ChangePasswordViewModel) })]
        [TestCase(nameof(AccountController.ChangeEmailAddress), new[] { typeof(ChangeEmailViewModel) })]
        [TestCase(nameof(AccountController.LinkLogin), new[] { typeof(string) })]
        [TestCase(nameof(AccountController.ExternalLoginConfirmation), new[] { typeof(ExternalLoginConfirmationViewModel), typeof(string) })]
        [TestCase(nameof(AccountController.LogOff), new Type[0])]
        [TestCase(nameof(AccountController.ForgotPassword), new[] { typeof(ForgotPasswordViewModel) })]
        [TestCase(nameof(AccountController.ResetPassword), new[] { typeof(ResetPasswordViewModel) })]
        public void ListedActionsUseHandledValidateAntiForgeryTokenForbiddenHtmlMode(string actionName, Type[] parameterTypes)
        {
            var actionMethod = GetAccountActionMethod(actionName, parameterTypes);

            var configuredAttribute = GetHandledValidateAntiForgeryTokenAttribute(actionMethod);

            Assert.That(configuredAttribute, Is.Not.Null, $"Expected {actionMethod.Name} to use HandledValidateAntiForgeryTokenAttribute.");
            Assert.That(GetFailureMode(configuredAttribute), Is.EqualTo(AntiForgeryFailureMode.ForbiddenHtml));
        }

        [Test]
        public void LogOffHandledAntiForgeryFailureReturns403AndSkipsSignOut()
        {
            var filterContext = CreateAuthorizationContext(accountControllerPartialMock, GetAccountActionMethod(nameof(AccountController.LogOff)));
            var testableAttribute = new TestableHandledValidateAntiForgeryTokenAttribute(AntiForgeryFailureMode.ForbiddenHtml)
            {
                ShouldThrow = true
            };

            var actionBodyExecuted = false;

            testableAttribute.OnAuthorization(filterContext);
            if (filterContext.Result == null)
            {
                actionBodyExecuted = true;
            }

            var result = filterContext.Result as HttpStatusCodeResult;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(403));
            Assert.That(actionBodyExecuted, Is.False);
            authenticationManagerMock.AssertWasNotCalled(mock => mock.SignOut());
        }

        [Test]
        public void ForgotPasswordHandledAntiForgeryFailureReturns403AndSkipsFindByEmail()
        {
            var testUserManager = CreateUserManagerDouble();
            var controller = CreateAccountController(testUserManager, authenticationManagerMock);
            var filterContext = CreateAuthorizationContext(controller, GetAccountActionMethod(nameof(AccountController.ForgotPassword), typeof(ForgotPasswordViewModel)));
            var testableAttribute = new TestableHandledValidateAntiForgeryTokenAttribute(AntiForgeryFailureMode.ForbiddenHtml)
            {
                ShouldThrow = true
            };

            var actionBodyExecuted = false;

            testableAttribute.OnAuthorization(filterContext);
            if (filterContext.Result == null)
            {
                actionBodyExecuted = true;
            }

            var result = filterContext.Result as HttpStatusCodeResult;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(403));
            Assert.That(actionBodyExecuted, Is.False);
            testUserManager.AssertWasNotCalled(mock => mock.FindByEmailAsync(Arg<string>.Is.Anything));
        }

        [Test]
        public void ExternalLoginConfirmationHandledAntiForgeryFailureReturns403AndSkipsCreateUser()
        {
            var testUserManager = CreateUserManagerDouble();
            var controller = CreateAccountController(testUserManager, authenticationManagerMock);
            var filterContext = CreateAuthorizationContext(controller, GetAccountActionMethod(nameof(AccountController.ExternalLoginConfirmation), typeof(ExternalLoginConfirmationViewModel), typeof(string)));
            var testableAttribute = new TestableHandledValidateAntiForgeryTokenAttribute(AntiForgeryFailureMode.ForbiddenHtml)
            {
                ShouldThrow = true
            };

            var actionBodyExecuted = false;

            testableAttribute.OnAuthorization(filterContext);
            if (filterContext.Result == null)
            {
                actionBodyExecuted = true;
            }

            var result = filterContext.Result as HttpStatusCodeResult;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(403));
            Assert.That(actionBodyExecuted, Is.False);
            testUserManager.AssertWasNotCalled(mock => mock.CreateAsync(Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void LogOffUnauthenticatedRequestKeepsAuthorizeResultInsteadOfFallingIntoAntiForgeryHandling()
        {
            var controller = CreateAccountController(userManager, authenticationManagerMock);
            var actionMethod = GetAccountActionMethod(nameof(AccountController.LogOff));
            var filterContext = CreateAuthorizationContext(controller, actionMethod, isAuthenticated: false);
            var authorizeAttribute = typeof(AccountController)
                .GetCustomAttributes(typeof(AuthorizeAttribute), true)
                .OfType<AuthorizeAttribute>()
                .Single();
            var antiForgeryAttribute = new TestableHandledValidateAntiForgeryTokenAttribute(AntiForgeryFailureMode.ForbiddenHtml)
            {
                ShouldThrow = true
            };

            Assert.That(authorizeAttribute, Is.Not.Null);

            filterContext.Result = new HttpUnauthorizedResult();
            antiForgeryAttribute.OnAuthorization(filterContext);

            Assert.That(filterContext.Result, Is.TypeOf<HttpUnauthorizedResult>());
            authenticationManagerMock.AssertWasNotCalled(mock => mock.SignOut());
        }

        private AccountController CreateAccountController(ApplicationUserManager testUserManager, Microsoft.Owin.Security.IAuthenticationManager testAuthenticationManager)
        {
            return new AccountController(
                testUserManager,
                userRegistererMock,
                firstTimeAuthenticatorMock,
                testAuthenticationManager,
                gamingGroupInviteConsumerMock,
                gamingGroupRetrieverMock,
                MockRepository.GenerateMock<BusinessLogic.Logic.Users.IBoardGameGeekUserSaver>(),
                MockRepository.GenerateMock<BoardGameGeekApiClient.Interfaces.IBoardGameGeekApiClient>(),
                MockRepository.GenerateMock<BusinessLogic.Logic.Users.IUserRetriever>(),
                transformerMock,
                gamingGroupContextSwitcher);
        }

        private static ApplicationUserManager CreateUserManagerDouble()
        {
            var userStore = MockRepository.GenerateMock<IUserStore<ApplicationUser>>();
            var dataProtector = MockRepository.GenerateMock<IDataProtector>();
            var dataProtectionProvider = MockRepository.GenerateMock<IDataProtectionProvider>();
            dataProtectionProvider.Expect(mock => mock.Create(Arg<string>.Is.Anything)).Return(dataProtector);

            return MockRepository.GeneratePartialMock<ApplicationUserManager>(userStore, dataProtectionProvider);
        }

        private AuthorizationContext CreateAuthorizationContext(Controller controller, MethodInfo actionMethod, bool isAuthenticated = true)
        {
            controller.TempData = new TempDataDictionary();

            var identity = MockRepository.GenerateStub<System.Security.Principal.IIdentity>();
            identity.Stub(x => x.IsAuthenticated).Return(isAuthenticated);

            var principal = MockRepository.GenerateStub<System.Security.Principal.IPrincipal>();
            principal.Stub(x => x.Identity).Return(identity);

            var form = new NameValueCollection();
            var request = MockRepository.GenerateStub<HttpRequestBase>();
            request.Stub(x => x.Form).Return(form);

            var response = MockRepository.GenerateStub<HttpResponseBase>();
            var httpContext = MockRepository.GenerateStub<HttpContextBase>();
            httpContext.Stub(x => x.Request).Return(request);
            httpContext.Stub(x => x.Response).Return(response);
            httpContext.User = principal;

            var routeData = new System.Web.Routing.RouteData();
            routeData.DataTokens["antiForgeryTest"] = null;
            var controllerContext = new ControllerContext(new System.Web.Routing.RequestContext(httpContext, routeData), controller);
            controller.ControllerContext = controllerContext;

            var actionDescriptor = new ReflectedActionDescriptor(actionMethod, actionMethod.Name, new ReflectedControllerDescriptor(controller.GetType()));

            return new AuthorizationContext(controllerContext, actionDescriptor);
        }

        private static MethodInfo GetAccountActionMethod(string actionName, params Type[] parameterTypes)
        {
            return typeof(AccountController).GetMethod(actionName, parameterTypes);
        }

        private static HandledValidateAntiForgeryTokenAttribute GetHandledValidateAntiForgeryTokenAttribute(MethodInfo actionMethod)
        {
            return actionMethod
                .GetCustomAttributes(typeof(HandledValidateAntiForgeryTokenAttribute), false)
                .OfType<HandledValidateAntiForgeryTokenAttribute>()
                .SingleOrDefault();
        }

        private static AntiForgeryFailureMode GetFailureMode(HandledValidateAntiForgeryTokenAttribute attribute)
        {
            var field = typeof(HandledValidateAntiForgeryTokenAttribute)
                .GetField("_failureMode", BindingFlags.Instance | BindingFlags.NonPublic);

            return (AntiForgeryFailureMode)field.GetValue(attribute);
        }

        private class TestableHandledValidateAntiForgeryTokenAttribute : HandledValidateAntiForgeryTokenAttribute
        {
            public bool ShouldThrow { get; set; }

            public TestableHandledValidateAntiForgeryTokenAttribute(AntiForgeryFailureMode failureMode) : base(failureMode)
            {
            }

            internal override void ValidateAntiForgeryToken()
            {
                if (ShouldThrow)
                {
                    throw new HttpAntiForgeryException("invalid anti-forgery token");
                }
            }
        }
    }
}
