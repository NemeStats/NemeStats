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

using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Attributes.Filters;

namespace UI.Tests.UnitTests.AttributesTests
{
    [TestFixture]
    public class HandledValidateAntiForgeryTokenAttributeTests
    {
        private AuthorizationContext _filterContext;
        private FakeController _controller;
        private HttpResponseBase _response;
        private NameValueCollection _form;

        [SetUp]
        public void SetUp()
        {
            _controller = new FakeController
            {
                TempData = new TempDataDictionary()
            };

            var httpContext = MockRepository.GenerateStub<HttpContextBase>();
            var request = MockRepository.GenerateStub<HttpRequestBase>();
            _response = MockRepository.GenerateMock<HttpResponseBase>();
            _response.Stub(x => x.TrySkipIisCustomErrors).PropertyBehavior();
            _form = new NameValueCollection();

            request.Stub(x => x.Form).Return(_form);
            httpContext.Stub(x => x.Request).Return(request);
            httpContext.Stub(x => x.Response).Return(_response);

            var controllerContext = new ControllerContext
            {
                Controller = _controller,
                HttpContext = httpContext
            };
            _controller.ControllerContext = controllerContext;

            _filterContext = new AuthorizationContext(controllerContext, MockRepository.GenerateStub<ActionDescriptor>());
        }

        [Test]
        public void ItDoesNothingWhenAntiForgeryValidationSucceeds()
        {
            var attribute = new TestableHandledValidateAntiForgeryTokenAttribute(AntiForgeryFailureMode.RetryLogin);

            attribute.OnAuthorization(_filterContext);

            Assert.That(_filterContext.Result, Is.Null);
            Assert.That(_controller.TempData.ContainsKey(HandledValidateAntiForgeryTokenAttribute.AntiForgeryErrorKey), Is.False);
        }

        [TestCase("home", "/")]
        [TestCase("login", "/Account/Login")]
        [TestCase("register", "/Account/Register")]
        [TestCase("something-else", "/Account/Login")]
        [TestCase(null, "/Account/Login")]
        public void RetryLoginRedirectsBasedOnOrigin(string origin, string expectedUrl)
        {
            var attribute = CreateFailingAttribute(AntiForgeryFailureMode.RetryLogin);
            _form["origin"] = origin;

            attribute.OnAuthorization(_filterContext);

            var result = _filterContext.Result as RedirectResult;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Url, Is.EqualTo(expectedUrl));
            AssertRetryTempData();
        }

        [Test]
        public void RetryRegisterRedirectsToInvitationWhenInvitationIdIsPresent()
        {
            var attribute = CreateFailingAttribute(AntiForgeryFailureMode.RetryRegister);
            _form["GamingGroupInvitationId"] = "invite-123";

            attribute.OnAuthorization(_filterContext);

            var result = _filterContext.Result as RedirectResult;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Url, Is.EqualTo("/Account/ConsumeInvitation?id=invite-123"));
            AssertRetryTempData();
        }

        [Test]
        public void RetryRegisterRedirectsToRegisterWhenInvitationIdIsMissing()
        {
            var attribute = CreateFailingAttribute(AntiForgeryFailureMode.RetryRegister);

            attribute.OnAuthorization(_filterContext);

            var result = _filterContext.Result as RedirectResult;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Url, Is.EqualTo("/Account/Register"));
            AssertRetryTempData();
        }

        [Test]
        public void ForbiddenHtmlReturns403()
        {
            var attribute = CreateFailingAttribute(AntiForgeryFailureMode.ForbiddenHtml);

            attribute.OnAuthorization(_filterContext);

            var result = _filterContext.Result as HttpStatusCodeResult;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(403));
        }

        [Test]
        public void ForbiddenJsonReturnsExpectedPayloadAndSkipsIisCustomErrors()
        {
            var attribute = CreateFailingAttribute(AntiForgeryFailureMode.ForbiddenJson);

            attribute.OnAuthorization(_filterContext);

            var result = _filterContext.Result as JsonResult;
            Assert.That(result, Is.Not.Null);
            Assert.That(_filterContext.HttpContext.Response.TrySkipIisCustomErrors, Is.True);
            Assert.That(result.JsonRequestBehavior, Is.EqualTo(JsonRequestBehavior.AllowGet));

            dynamic data = result.Data;
            Assert.That(data.success, Is.False);
            Assert.That(data.message, Is.EqualTo("This page expired. Please try again."));
        }

        private static TestableHandledValidateAntiForgeryTokenAttribute CreateFailingAttribute(AntiForgeryFailureMode failureMode)
        {
            return new TestableHandledValidateAntiForgeryTokenAttribute(failureMode)
            {
                ShouldThrow = true
            };
        }

        private void AssertRetryTempData()
        {
            Assert.That(_controller.TempData[HandledValidateAntiForgeryTokenAttribute.AntiForgeryErrorKey], Is.EqualTo("This page expired. Please try again."));
            Assert.That(_controller.TempData[HandledValidateAntiForgeryTokenAttribute.AntiForgeryErrorKindKey], Is.EqualTo("warning"));
        }

        private class FakeController : Controller
        {
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
