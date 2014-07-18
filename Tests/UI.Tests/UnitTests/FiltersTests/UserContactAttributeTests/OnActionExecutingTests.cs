using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UI.Filters;

namespace UI.Tests.UnitTests.FiltersTests.UserNameActionFilterTests
{
    [TestFixture]
    public class OnActionExecutingTests
    {
        private ActionExecutingContext actionExecutingContext;
        private UserContextAttribute userContextActionFilter;
        private IIdentity identity;
        private UserContext userContext;
        private NemeStatsDbContext dbContextMock;

        [SetUp]
        public void SetUp()
        {
            actionExecutingContext = new ActionExecutingContext();
            actionExecutingContext.ActionParameters = new Dictionary<string, object>();
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

            userContextActionFilter = new UserContextAttribute();
            dbContextMock = MockRepository.GenerateMock<NemeStatsDbContext>();

            UserContextBuilder userContextBuilderMock = MockRepository.GenerateMock<UserContextBuilder>();
            userContext = new UserContext()
            {
                ApplicationUserId = "user id",
                GamingGroupId = 135151
            };
            userContextBuilderMock.Expect(mock => mock.GetUserContext(Arg<string>.Is.Anything, Arg<NemeStatsDbContext>.Is.Same(dbContextMock)))
                .Repeat.Once()
                .Return(userContext);

            userContextActionFilter.userContextBuilder = userContextBuilderMock;
        }

        [Test]
        public void ItThrowsAnInvalidOperationExceptionIfTheUserIsNotAuthenticated()
        {
            identity.BackToRecord(BackToRecordOptions.All);
            identity.Expect(mock => mock.IsAuthenticated)
                .Repeat.Once()
                .Return(false);

            var exception = Assert.Throws<InvalidOperationException>(() => userContextActionFilter.OnActionExecuting(actionExecutingContext, dbContextMock));
            Assert.AreEqual(UserContextAttribute.EXCEPTION_MESSAGE_USER_NOT_AUTHENTICATED, exception.Message);
        }

        [Test]
        public void ItSetsTheUserConextActionParameterIfItIsntAlreadySet()
        {
            userContextActionFilter.OnActionExecuting(actionExecutingContext, dbContextMock);

            Assert.AreEqual(userContext, actionExecutingContext.ActionParameters[UserContextAttribute.USER_CONTEXT_KEY]);
        }

        [Test]
        public void IfAGamingGroupIsRequiredAndUserDoesntHaveAGaminGroupItRedirectsUserToTheCreateAction()
        {
            userContextActionFilter.RequiresGamingGroup = true;
            userContext.GamingGroupId = null;

            userContextActionFilter.OnActionExecuting(actionExecutingContext, dbContextMock);

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
            userContext.GamingGroupId = 1;

            userContextActionFilter.OnActionExecuting(actionExecutingContext, dbContextMock);

            Assert.Null(actionExecutingContext.Result);
        }

        [Test]
        public void ItDoesntRedirectIfGamingGroupIsNotRequired()
        {
            userContextActionFilter.RequiresGamingGroup = false;
            userContext.GamingGroupId = null;

            userContextActionFilter.OnActionExecuting(actionExecutingContext, dbContextMock);

            Assert.Null(actionExecutingContext.Result);
        }
    }
}
