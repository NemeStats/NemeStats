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
using BusinessLogic.EventTracking;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UI.Filters;

namespace UI.Tests.UnitTests.FiltersTests.UserContextAttributeTests
{
    [TestFixture]
    public class OnActionExecutingTests
    {
        private ActionExecutingContext actionExecutingContext;
        private UserContextAttribute userContextActionFilter;
        private IIdentity identity;
        private ApplicationUserManager userManager;
        private IUserStore<ApplicationUser> userStoreMock;
        private ApplicationUser applicationUser;
        private string anonymousClientId = "anonymous client id";
        private NameValueCollection requestParameters;

        [SetUp]
        public void SetUp()
        {
            actionExecutingContext = new ActionExecutingContext();
            actionExecutingContext.ActionParameters = new Dictionary<string, object>();
            userStoreMock = MockRepository.GenerateMock<IUserStore<ApplicationUser>>();
            userManager = new ApplicationUserManager(userStoreMock);
            //need to simulate like the parameter exists on the method
            actionExecutingContext.ActionParameters[UserContextAttribute.USER_CONTEXT_KEY] = null;

            HttpContextBase httpContextBase = MockRepository.GenerateMock<HttpContextBase>();
            actionExecutingContext.HttpContext = httpContextBase;

            IPrincipal principal = MockRepository.GenerateMock<IPrincipal>();
            httpContextBase.Expect(contextBase => contextBase.User)
                .Repeat.Any()
                .Return(principal);
            identity = MockRepository.GenerateMock<IIdentity>();
            principal.Expect(mock => mock.Identity)
                .Repeat.Any()
                .Return(identity);
            identity.Expect(mock => mock.IsAuthenticated)
                .Repeat.Once()
                .Return(true);

            HttpRequestBase requestBaseMock = MockRepository.GenerateMock<HttpRequestBase>();

            httpContextBase.Expect(mock => mock.Request)
                .Return(requestBaseMock);
            requestParameters = new NameValueCollection();
            requestParameters.Add(UserContextAttribute.REQUEST_PARAM_ANALYTICS_ID, anonymousClientId);
            requestBaseMock.Expect(mock => mock.Params)
                .Return(requestParameters);

            userContextActionFilter = new UserContextAttribute();
            applicationUser = new ApplicationUser()
            {
                Id = "user id",
                CurrentGamingGroupId = 315
            };
            Task<ApplicationUser> task = Task.FromResult(applicationUser);
            //TODO can't figure out how to mock the GetUserId() extension method, so have to be less strict here
            userStoreMock.Expect(mock => mock.FindByIdAsync(Arg<string>.Is.Anything))
                .Repeat.Once()
                .Return(task);
        }

        [Test]
        public void ItUsesTheAnonymousUserIfTheUserIsNotAuthenticated()
        {
            identity.BackToRecord(BackToRecordOptions.All);
            identity.Expect(mock => mock.IsAuthenticated)
                .Repeat.Once()
                .Return(false);

            userContextActionFilter.OnActionExecuting(actionExecutingContext, userManager);
            Assert.IsInstanceOf<AnonymousApplicationUser>(actionExecutingContext.ActionParameters[UserContextAttribute.USER_CONTEXT_KEY]);
        }

        [Test]
        public void ItSetsTheUserConextActionParameterIfItIsntAlreadySet()
        {
            userContextActionFilter.OnActionExecuting(actionExecutingContext, userManager);

            Assert.AreEqual(applicationUser, actionExecutingContext.ActionParameters[UserContextAttribute.USER_CONTEXT_KEY]);
        }

        [Test]
        public void IfAGamingGroupIsRequiredAndUserDoesntHaveAGaminGroupItRedirectsUserToTheLoginAction()
        {
            userContextActionFilter.RequiresGamingGroup = true;
            applicationUser.CurrentGamingGroupId = null;

            userContextActionFilter.OnActionExecuting(actionExecutingContext, userManager);

            RouteValueDictionary dictionary = new RouteValueDictionary();
            dictionary.Add("Area", "");
            dictionary.Add("Controller", "Account");
            dictionary.Add("Action", "Login");
            RedirectToRouteResult actualResult = (RedirectToRouteResult)actionExecutingContext.Result;
            Assert.AreEqual(dictionary, actualResult.RouteValues);
        }

        [Test]
        public void ItDoesntRedirectIfTheUserHasAGamingGroup()
        {
            userContextActionFilter.RequiresGamingGroup = true;
            applicationUser.CurrentGamingGroupId = 1;

            userContextActionFilter.OnActionExecuting(actionExecutingContext, userManager);

            Assert.Null(actionExecutingContext.Result);
        }

        [Test]
        public void ItDoesntRedirectIfGamingGroupIsNotRequired()
        {
            userContextActionFilter.RequiresGamingGroup = false;
            applicationUser.CurrentGamingGroupId = null;

            userContextActionFilter.OnActionExecuting(actionExecutingContext, userManager);

            Assert.Null(actionExecutingContext.Result);
        }

        [Test]
        public void ItAddsTheAnonymousClientIdToTheUserIfItExists()
        {
            userContextActionFilter.OnActionExecuting(actionExecutingContext, userManager);

            ApplicationUser actualApplicationUser = (ApplicationUser)actionExecutingContext.ActionParameters[UserContextAttribute.USER_CONTEXT_KEY];
            Assert.AreEqual(applicationUser.AnonymousClientId, actualApplicationUser.AnonymousClientId);
        }

        [Test]
        public void ItSetsTheApplicationUsersAnonymousClientIdToSomeConstantIfItCannotBePulledFromTheCookie()
        {
            requestParameters.Clear();
            userContextActionFilter.OnActionExecuting(actionExecutingContext, userManager);

            ApplicationUser actualApplicationUser = (ApplicationUser)actionExecutingContext.ActionParameters[UserContextAttribute.USER_CONTEXT_KEY];
            Assert.AreEqual(UniversalAnalyticsNemeStatsEventTracker.DEFAULT_ANONYMOUS_CLIENT_ID, actualApplicationUser.AnonymousClientId);
        }
    }
}
