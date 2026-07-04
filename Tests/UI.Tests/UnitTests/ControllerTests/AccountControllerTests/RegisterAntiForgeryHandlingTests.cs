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
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Attributes.Filters;
using UI.Controllers;
using UI.Models;

namespace UI.Tests.UnitTests.ControllerTests.AccountControllerTests
{
    [TestFixture]
    public class RegisterAntiForgeryHandlingTests : AccountControllerTestBase
    {
        [Test]
        public void RegisterActionUsesHandledValidateAntiForgeryTokenRetryRegisterMode()
        {
            var actionMethod = GetAccountActionMethod(nameof(AccountController.Register), typeof(RegisterViewModel));

            var configuredAttribute = GetHandledValidateAntiForgeryTokenAttribute(actionMethod);

            Assert.That(configuredAttribute, Is.Not.Null, $"Expected {actionMethod.Name} to use HandledValidateAntiForgeryTokenAttribute.");
            Assert.That(GetFailureMode(configuredAttribute), Is.EqualTo(AntiForgeryFailureMode.RetryRegister));
        }

        [Test]
        public void RegisterHandledAntiForgeryFailureWithoutInvitationRedirectsBackToRegisterAndSkipsActionExecution()
        {
            AssertHandledRetryRegisterFailure(null, "/Account/Register");
        }

        [Test]
        public void RegisterHandledAntiForgeryFailureWithInvitationRedirectsBackToConsumeInvitationAndSkipsActionExecution()
        {
            var invitationId = Guid.NewGuid().ToString();

            AssertHandledRetryRegisterFailure(invitationId, $"/Account/ConsumeInvitation?id={invitationId}");
        }

        private void AssertHandledRetryRegisterFailure(string gamingGroupInvitationId, string expectedUrl)
        {
            var filterContext = CreateAuthorizationContext(gamingGroupInvitationId);
            var testableAttribute = new TestableHandledValidateAntiForgeryTokenAttribute(AntiForgeryFailureMode.RetryRegister)
            {
                ShouldThrow = true
            };

            var actionBodyExecuted = false;

            testableAttribute.OnAuthorization(filterContext);
            if (filterContext.Result == null)
            {
                actionBodyExecuted = true;
            }

            var result = filterContext.Result as RedirectResult;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Url, Is.EqualTo(expectedUrl));
            Assert.That(actionBodyExecuted, Is.False);
            Assert.That(accountControllerPartialMock.TempData[HandledValidateAntiForgeryTokenAttribute.AntiForgeryErrorKey], Is.EqualTo("This page expired. Please try again."));
            Assert.That(accountControllerPartialMock.TempData[HandledValidateAntiForgeryTokenAttribute.AntiForgeryErrorKindKey], Is.EqualTo("warning"));
            userRegistererMock.AssertWasNotCalled(mock => mock.RegisterUser(Arg<NewUser>.Is.Anything));
        }

        private AuthorizationContext CreateAuthorizationContext(string gamingGroupInvitationId)
        {
            accountControllerPartialMock.TempData = new TempDataDictionary();

            var form = new NameValueCollection();
            if (!string.IsNullOrWhiteSpace(gamingGroupInvitationId))
            {
                form["GamingGroupInvitationId"] = gamingGroupInvitationId;
            }

            var request = MockRepository.GenerateStub<HttpRequestBase>();
            request.Stub(x => x.Form).Return(form);

            var response = MockRepository.GenerateStub<HttpResponseBase>();
            var httpContext = MockRepository.GenerateStub<HttpContextBase>();
            httpContext.Stub(x => x.Request).Return(request);
            httpContext.Stub(x => x.Response).Return(response);

            var controllerContext = new ControllerContext
            {
                Controller = accountControllerPartialMock,
                HttpContext = httpContext
            };
            accountControllerPartialMock.ControllerContext = controllerContext;

            return new AuthorizationContext(controllerContext, MockRepository.GenerateStub<ActionDescriptor>());
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
