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

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.DataProtection;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Attributes;
using UI.Attributes.Filters;

namespace UI.Tests.UnitTests.AttributesTests.FiltersTests.UserContextAttributeTests
{
    [TestFixture]
    public class OnActionExecutingTests
    {
        private ActionExecutingContext actionExecutingContext;
        private UserContextAttribute userContextActionFilter;
        private IIdentity identity;
        private ApplicationUserManager userManager;
        private IUserStore<ApplicationUser> userStoreMock;
        private IDataProtectionProvider dataProtectionProviderMock;
        private ClientIdCalculator clientIdCalculatorMock;
        private ApplicationUser applicationUser;
        private NameValueCollection requestParameters;

        [SetUp]
        public void SetUp()
        {
            this.actionExecutingContext = new ActionExecutingContext
            {
                ActionParameters = new Dictionary<string, object>()
            };
            this.userStoreMock = MockRepository.GenerateMock<IUserStore<ApplicationUser>>();
            this.dataProtectionProviderMock = MockRepository.GenerateMock<IDataProtectionProvider>();
            var dataProtector = MockRepository.GenerateMock<IDataProtector>();
            this.dataProtectionProviderMock.Expect(mock => mock.Create(Arg<string>.Is.Anything)).Return(dataProtector);
            this.userManager = new ApplicationUserManager(this.userStoreMock, this.dataProtectionProviderMock);
            clientIdCalculatorMock = MockRepository.GenerateMock<ClientIdCalculator>();
            //need to simulate like the parameter exists on the method
            this.actionExecutingContext.ActionParameters[UserContextAttribute.UserContextKey] = null;

            HttpContextBase httpContextBase = MockRepository.GenerateMock<HttpContextBase>();
            this.actionExecutingContext.HttpContext = httpContextBase;

            IPrincipal principal = MockRepository.GenerateMock<IPrincipal>();
            httpContextBase.Expect(contextBase => contextBase.User)
                .Repeat.Any()
                .Return(principal);
            this.identity = MockRepository.GenerateMock<IIdentity>();
            principal.Expect(mock => mock.Identity)
                .Repeat.Any()
                .Return(this.identity);
            this.identity.Expect(mock => mock.IsAuthenticated)
                .Repeat.Once()
                .Return(true);

            HttpRequestBase requestBaseMock = MockRepository.GenerateMock<HttpRequestBase>();

            httpContextBase.Expect(mock => mock.Request)
                .Return(requestBaseMock);
            this.requestParameters = new NameValueCollection();
            requestBaseMock.Expect(mock => mock.Params)
                .Return(this.requestParameters);

            this.userContextActionFilter = new UserContextAttribute();
            this.applicationUser = new ApplicationUser()
            {
                Id = "user id",
                CurrentGamingGroupId = 315
            };
            Task<ApplicationUser> task = Task.FromResult(this.applicationUser);
            //TODO can't figure out how to mock the GetUserId() extension method, so have to be less strict here
            this.userStoreMock.Expect(mock => mock.FindByIdAsync(Arg<string>.Is.Anything))
                .Repeat.Once()
                .Return(task);
        }

        [Test]
        public void ItUsesTheAnonymousUserIfTheUserIsNotAuthenticated()
        {
            this.identity.BackToRecord(BackToRecordOptions.All);
            this.identity.Expect(mock => mock.IsAuthenticated)
                .Repeat.Once()
                .Return(false);

            this.userContextActionFilter.OnActionExecuting(this.actionExecutingContext, this.userManager, this.clientIdCalculatorMock);
            Assert.IsInstanceOf<AnonymousApplicationUser>(this.actionExecutingContext.ActionParameters[UserContextAttribute.UserContextKey]);
        }

        [Test]
        public void ItSetsTheUserConextActionParameterIfItIsntAlreadySet()
        {
            this.userContextActionFilter.OnActionExecuting(this.actionExecutingContext, this.userManager, this.clientIdCalculatorMock);

            Assert.AreEqual(this.applicationUser, this.actionExecutingContext.ActionParameters[UserContextAttribute.UserContextKey]);
        }

        [Test]
        public void IfAGamingGroupIsRequiredAndUserIsAnonymousItRedirectsUserToTheLoginAction()
        {
            this.userContextActionFilter.RequiresGamingGroup = true;
            this.identity.BackToRecord(BackToRecordOptions.All);
            this.identity.Expect(mock => mock.IsAuthenticated)
                .Repeat.Once()
                .Return(false);

            this.userContextActionFilter.OnActionExecuting(this.actionExecutingContext, this.userManager, this.clientIdCalculatorMock);

            RouteValueDictionary dictionary = new RouteValueDictionary();
            dictionary.Add("Area", "");
            dictionary.Add("Controller", "Account");
            dictionary.Add("Action", "Login");
            RedirectToRouteResult actualResult = (RedirectToRouteResult)this.actionExecutingContext.Result;
            Assert.AreEqual(dictionary, actualResult.RouteValues);
        }

        [Test]
        public void ItDoesntRedirectIfTheUserHasAGamingGroup()
        {
            this.userContextActionFilter.RequiresGamingGroup = true;
            this.applicationUser.CurrentGamingGroupId = 1;

            this.userContextActionFilter.OnActionExecuting(this.actionExecutingContext, this.userManager, this.clientIdCalculatorMock);

            Assert.Null(this.actionExecutingContext.Result);
        }

        [Test]
        public void ItDoesntRedirectIfGamingGroupIsNotRequired()
        {
            this.userContextActionFilter.RequiresGamingGroup = false;
            this.identity.BackToRecord(BackToRecordOptions.All);
            this.identity.Expect(mock => mock.IsAuthenticated)
                .Repeat.Once()
                .Return(false);

            this.userContextActionFilter.OnActionExecuting(this.actionExecutingContext, this.userManager, this.clientIdCalculatorMock);

            Assert.Null(this.actionExecutingContext.Result);
        }

        [Test]
        public void ItAddsTheAnonymousClientIdToTheUserIfItExists()
        {
            const string EXPECTED_CLIENT_ID = "some client id";
            clientIdCalculatorMock.Expect(mock => mock.GetClientId(actionExecutingContext.HttpContext.Request, applicationUser)).Return(EXPECTED_CLIENT_ID);

            this.userContextActionFilter.OnActionExecuting(this.actionExecutingContext, this.userManager, this.clientIdCalculatorMock);

            ApplicationUser actualApplicationUser = (ApplicationUser)this.actionExecutingContext.ActionParameters[UserContextAttribute.UserContextKey];
            Assert.That(actualApplicationUser.AnonymousClientId, Is.EqualTo(EXPECTED_CLIENT_ID));
        }
    }
}
