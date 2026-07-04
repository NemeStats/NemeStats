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
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using BusinessLogic.Logic.GameDefinitions;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Attributes.Filters;
using UI.Controllers;
using UI.Models.GameDefinitionModels;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
    [TestFixture]
    public class GameDefinitionControllerAntiForgeryTests : GameDefinitionControllerTestBase
    {
        [TestCase(nameof(GameDefinitionController.Create), new[] { typeof(CreateGameDefinitionViewModel), typeof(BusinessLogic.Models.User.ApplicationUser) })]
        [TestCase(nameof(GameDefinitionController.Edit), new[] { typeof(GameDefinitionEditViewModel), typeof(BusinessLogic.Models.User.ApplicationUser) })]
        public void ListedActionsUseHandledValidateAntiForgeryTokenForbiddenHtmlMode(string actionName, Type[] parameterTypes)
        {
            var actionMethod = GetGameDefinitionActionMethod(actionName, parameterTypes);

            var configuredAttribute = GetHandledValidateAntiForgeryTokenAttribute(actionMethod);

            Assert.That(configuredAttribute, Is.Not.Null, $"Expected {actionMethod.Name} to use HandledValidateAntiForgeryTokenAttribute.");
            Assert.That(GetFailureMode(configuredAttribute), Is.EqualTo(AntiForgeryFailureMode.ForbiddenHtml));
        }

        [Test]
        public void EditHandledAntiForgeryFailureReturns403AndSkipsUpdateGameDefinition()
        {
            var actionMethod = GetGameDefinitionActionMethod(nameof(GameDefinitionController.Edit), typeof(GameDefinitionEditViewModel), typeof(BusinessLogic.Models.User.ApplicationUser));
            var filterContext = CreateAuthorizationContext(actionMethod, isAuthenticated: true);
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
            autoMocker.Get<IGameDefinitionSaver>().AssertWasNotCalled(mock => mock.UpdateGameDefinition(Arg<GameDefinitionUpdateRequest>.Is.Anything, Arg<BusinessLogic.Models.User.ApplicationUser>.Is.Anything));
        }

        [Test]
        public void EditUnauthenticatedRequestKeepsAuthorizeResultInsteadOfFallingIntoAntiForgeryHandling()
        {
            var actionMethod = GetGameDefinitionActionMethod(nameof(GameDefinitionController.Edit), typeof(GameDefinitionEditViewModel), typeof(BusinessLogic.Models.User.ApplicationUser));
            var filterContext = CreateAuthorizationContext(actionMethod, isAuthenticated: false);
            var authorizeAttribute = actionMethod
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
            autoMocker.Get<IGameDefinitionSaver>().AssertWasNotCalled(mock => mock.UpdateGameDefinition(Arg<GameDefinitionUpdateRequest>.Is.Anything, Arg<BusinessLogic.Models.User.ApplicationUser>.Is.Anything));
        }

        private AuthorizationContext CreateAuthorizationContext(MethodInfo actionMethod, bool isAuthenticated)
        {
            var identity = MockRepository.GenerateStub<System.Security.Principal.IIdentity>();
            identity.Stub(x => x.IsAuthenticated).Return(isAuthenticated);

            var principal = MockRepository.GenerateStub<System.Security.Principal.IPrincipal>();
            principal.Stub(x => x.Identity).Return(identity);

            var httpContext = MockRepository.GenerateStub<HttpContextBase>();
            httpContext.Stub(x => x.Request).Return(asyncRequestMock);
            httpContext.Stub(x => x.Response).Return(MockRepository.GenerateStub<HttpResponseBase>());
            httpContext.User = principal;

            var routeData = new System.Web.Routing.RouteData();
            routeData.DataTokens["antiForgeryTest"] = null;
            var controllerContext = new ControllerContext(new System.Web.Routing.RequestContext(httpContext, routeData), autoMocker.ClassUnderTest);
            autoMocker.ClassUnderTest.ControllerContext = controllerContext;

            var actionDescriptor = new ReflectedActionDescriptor(actionMethod, actionMethod.Name, new ReflectedControllerDescriptor(typeof(GameDefinitionController)));

            return new AuthorizationContext(controllerContext, actionDescriptor);
        }

        private static MethodInfo GetGameDefinitionActionMethod(string actionName, params Type[] parameterTypes)
        {
            return typeof(GameDefinitionController).GetMethod(actionName, parameterTypes);
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
