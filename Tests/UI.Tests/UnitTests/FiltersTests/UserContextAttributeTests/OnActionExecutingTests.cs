using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using NUnit.Framework;
using Rhino.Mocks;
using System;
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
        public void IfAGamingGroupIsRequiredAndUserDoesntHaveAGaminGroupItRedirectsUserToTheCreateAction()
        {
            userContextActionFilter.RequiresGamingGroup = true;
            applicationUser.CurrentGamingGroupId = null;

            userContextActionFilter.OnActionExecuting(actionExecutingContext, userManager);

            RouteValueDictionary dictionary = new RouteValueDictionary();
            dictionary.Add("Area", "");
            dictionary.Add("Controller", "GamingGroup");
            dictionary.Add("Action", "Create");
            //TODO can't get the GetRouteValueDictionary extension method to work here  
            //new RedirectToRouteResult(MVC.GamingGroup.Create().GetRouteValueDictionary());
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
