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

using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Attributes.Filters;
using UI.Controllers;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
    [TestFixture]
    public class PlayedGameControllerAntiForgeryTests : PlayedGameControllerTestBase
    {
        [Test]
        public void SaveUsesHandledValidateAntiForgeryTokenForbiddenJsonMode()
        {
            var actionMethod = GetPlayedGameActionMethod();

            var configuredAttribute = GetHandledValidateAntiForgeryTokenAttribute(actionMethod);

            Assert.That(configuredAttribute, Is.Not.Null, $"Expected {actionMethod.Name} to use HandledValidateAntiForgeryTokenAttribute.");
            Assert.That(GetFailureMode(configuredAttribute), Is.EqualTo(AntiForgeryFailureMode.ForbiddenJson));
        }

        [Test]
        public void SaveHandledAntiForgeryFailureReturns403JsonAndSkipsCreatePlayedGame()
        {
            var actionMethod = GetPlayedGameActionMethod();
            var filterContext = CreateAuthorizationContext(actionMethod, isAuthenticated: true);
            var testableAttribute = new TestableHandledValidateAntiForgeryTokenAttribute(AntiForgeryFailureMode.ForbiddenJson)
            {
                ShouldThrow = true
            };

            var actionBodyExecuted = false;

            testableAttribute.OnAuthorization(filterContext);
            if (filterContext.Result == null)
            {
                actionBodyExecuted = true;
            }

            var result = filterContext.Result as JsonResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(filterContext.HttpContext.Response.StatusCode, Is.EqualTo(403));
            Assert.That(filterContext.HttpContext.Response.TrySkipIisCustomErrors, Is.True);
            Assert.That(result.JsonRequestBehavior, Is.EqualTo(JsonRequestBehavior.AllowGet));

            dynamic data = result.Data;
            Assert.That(data.success, Is.False);
            Assert.That(data.message, Is.EqualTo("This page expired. Please try again."));
            Assert.That(actionBodyExecuted, Is.False);
            AutoMocker.Get<ICreatePlayedGameComponent>().AssertWasNotCalled(mock => mock.Execute(Arg<BusinessLogic.Models.Games.NewlyCompletedGame>.Is.Anything, Arg<ApplicationUser>.Is.Anything));
        }

        private AuthorizationContext CreateAuthorizationContext(MethodInfo actionMethod, bool isAuthenticated)
        {
            var identity = MockRepository.GenerateStub<System.Security.Principal.IIdentity>();
            identity.Stub(x => x.IsAuthenticated).Return(isAuthenticated);

            var principal = MockRepository.GenerateStub<System.Security.Principal.IPrincipal>();
            principal.Stub(x => x.Identity).Return(identity);

            var response = MockRepository.GenerateMock<HttpResponseBase>();
            response.Stub(x => x.StatusCode).PropertyBehavior();
            response.Stub(x => x.TrySkipIisCustomErrors).PropertyBehavior();

            var httpContext = MockRepository.GenerateStub<HttpContextBase>();
            httpContext.Stub(x => x.Request).Return(AutoMocker.Get<HttpRequestBase>());
            httpContext.Stub(x => x.Response).Return(response);
            httpContext.User = principal;

            var routeData = new System.Web.Routing.RouteData();
            routeData.DataTokens["antiForgeryTest"] = null;
            var controllerContext = new ControllerContext(new System.Web.Routing.RequestContext(httpContext, routeData), AutoMocker.ClassUnderTest);
            AutoMocker.ClassUnderTest.ControllerContext = controllerContext;

            var actionDescriptor = new ReflectedActionDescriptor(actionMethod, actionMethod.Name, new ReflectedControllerDescriptor(typeof(PlayedGameController)));

            return new AuthorizationContext(controllerContext, actionDescriptor);
        }

        private static MethodInfo GetPlayedGameActionMethod()
        {
            return typeof(PlayedGameController).GetMethod(nameof(PlayedGameController.Save), new[] { typeof(SavePlayedGameRequest), typeof(ApplicationUser) });
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
