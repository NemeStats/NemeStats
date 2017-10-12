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
        private ActionExecutingContext _actionExecutingContext;
        private UserContextAttribute _userContextActionFilter;
        private IIdentity _identity;
        private ApplicationUserManager _userManager;
        private IUserStore<ApplicationUser> _userStoreMock;
        private IDataProtectionProvider _dataProtectionProviderMock;
        private ClientIdCalculator _clientIdCalculatorMock;
        private ApplicationUser _applicationUser;
        private NameValueCollection _requestParameters;

        private void SetupExpectations(bool isAuthenticated = true, bool userHasGamingGroup = true)
        {
            _actionExecutingContext = new ActionExecutingContext
            {
                ActionParameters = new Dictionary<string, object>()
            };
            _userStoreMock = MockRepository.GenerateMock<IUserStore<ApplicationUser>>();
            _dataProtectionProviderMock = MockRepository.GenerateMock<IDataProtectionProvider>();
            var dataProtector = MockRepository.GenerateMock<IDataProtector>();
            _dataProtectionProviderMock.Expect(mock => mock.Create(Arg<string>.Is.Anything)).Return(dataProtector);
            _userManager = new ApplicationUserManager(_userStoreMock, _dataProtectionProviderMock);
            _clientIdCalculatorMock = MockRepository.GenerateMock<ClientIdCalculator>();
            //need to simulate like the parameter exists on the method
            _actionExecutingContext.ActionParameters[UserContextAttribute.UserContextKey] = null;

            HttpContextBase httpContextBase = MockRepository.GenerateMock<HttpContextBase>();
            _actionExecutingContext.HttpContext = httpContextBase;

            IPrincipal principal = MockRepository.GenerateMock<IPrincipal>();
            httpContextBase.Expect(contextBase => contextBase.User)
                .Repeat.Any()
                .Return(principal);
            _identity = MockRepository.GenerateMock<IIdentity>();
            principal.Expect(mock => mock.Identity)
                .Repeat.Any()
                .Return(_identity);
            _identity.Expect(mock => mock.IsAuthenticated)
                .Repeat.Once()
                .Return(isAuthenticated);

            HttpRequestBase requestBaseMock = MockRepository.GenerateMock<HttpRequestBase>();

            httpContextBase.Expect(mock => mock.Request)
                .Return(requestBaseMock);
            _requestParameters = new NameValueCollection();
            requestBaseMock.Expect(mock => mock.Params)
                .Return(_requestParameters);

            _userContextActionFilter = new UserContextAttribute();
            _applicationUser = new ApplicationUser()
            {
                Id = "user id",
                CurrentGamingGroupId = userHasGamingGroup ? (int?)315 : null
            };
            Task<ApplicationUser> task = Task.FromResult(_applicationUser);
            //TODO can't figure out how to mock the GetUserId() extension method, so have to be less strict here
            _userStoreMock.Expect(mock => mock.FindByIdAsync(Arg<string>.Is.Anything))
                .Repeat.Once()
                .Return(task);
        }

        [Test]
        public void ItUsesTheAnonymousUserIfTheUserIsNotAuthenticated()
        {
            SetupExpectations(isAuthenticated: false);

            _userContextActionFilter.OnActionExecuting(_actionExecutingContext, _userManager, _clientIdCalculatorMock);
            Assert.IsInstanceOf<AnonymousApplicationUser>(_actionExecutingContext.ActionParameters[UserContextAttribute.UserContextKey]);
        }

        [Test]
        public void ItSetsTheUserConextActionParameterIfItIsntAlreadySet()
        {
            SetupExpectations();

            _userContextActionFilter.OnActionExecuting(_actionExecutingContext, _userManager, _clientIdCalculatorMock);

            Assert.AreEqual(_applicationUser, _actionExecutingContext.ActionParameters[UserContextAttribute.UserContextKey]);
        }

        [Test]
        public void IfAGamingGroupIsRequiredAndUserIsAnonymousItRedirectsUserToTheLoginAction()
        {
            SetupExpectations(isAuthenticated: false);
            _userContextActionFilter.RequiresGamingGroup = true;

            _userContextActionFilter.OnActionExecuting(_actionExecutingContext, _userManager, _clientIdCalculatorMock);

            RouteValueDictionary dictionary = new RouteValueDictionary();
            dictionary.Add("Area", "");
            dictionary.Add("Controller", "Account");
            dictionary.Add("Action", "Login");
            RedirectToRouteResult actualResult = (RedirectToRouteResult)_actionExecutingContext.Result;
            Assert.AreEqual(dictionary, actualResult.RouteValues);
        }

        [Test]
        public void IfAGamingGroupIsRequiredAndUserIsAuthenticatedWithNoCurrentGamingGroupItRedirectsUserToTheManageAccountAction()
        {
            SetupExpectations(userHasGamingGroup: false);
            _userContextActionFilter.RequiresGamingGroup = true;

            _userContextActionFilter.OnActionExecuting(_actionExecutingContext, _userManager, _clientIdCalculatorMock);

            RouteValueDictionary dictionary = new RouteValueDictionary();
            dictionary.Add("Area", "");
            dictionary.Add("Controller", "Account");
            dictionary.Add("Action", "Manage");
            RedirectToRouteResult actualResult = (RedirectToRouteResult)_actionExecutingContext.Result;
            Assert.AreEqual(dictionary, actualResult.RouteValues);
        }

        [Test]
        public void ItDoesntRedirectIfTheUserHasAGamingGroup()
        {
            SetupExpectations();
            _userContextActionFilter.RequiresGamingGroup = true;
            _applicationUser.CurrentGamingGroupId = 1;

            _userContextActionFilter.OnActionExecuting(_actionExecutingContext, _userManager, _clientIdCalculatorMock);

            Assert.Null(_actionExecutingContext.Result);
        }

        [Test]
        public void ItDoesntRedirectIfGamingGroupIsNotRequired()
        {
            SetupExpectations(isAuthenticated: false);

            _userContextActionFilter.RequiresGamingGroup = false;
            _userContextActionFilter.OnActionExecuting(_actionExecutingContext, _userManager, _clientIdCalculatorMock);

            Assert.Null(_actionExecutingContext.Result);
        }

        [Test]
        public void ItAddsTheAnonymousClientIdToTheUserIfItExists()
        {
            SetupExpectations();
            const string expectedClientId = "some client id";
            _clientIdCalculatorMock.Expect(mock => mock.GetClientId(_actionExecutingContext.HttpContext.Request, _applicationUser)).Return(expectedClientId);

            _userContextActionFilter.OnActionExecuting(_actionExecutingContext, _userManager, _clientIdCalculatorMock);

            ApplicationUser actualApplicationUser = (ApplicationUser)_actionExecutingContext.ActionParameters[UserContextAttribute.UserContextKey];
            Assert.That(actualApplicationUser.AnonymousClientId, Is.EqualTo(expectedClientId));
        }
    }
}
