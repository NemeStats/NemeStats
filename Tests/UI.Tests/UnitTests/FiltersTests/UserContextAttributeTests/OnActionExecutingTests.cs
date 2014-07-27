using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
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
        private UserManager<ApplicationUser> userManager;
        private IUserStore<ApplicationUser> userStoreMock;
        private ApplicationUser applicationUser;

        [SetUp]
        public void SetUp()
        {
            actionExecutingContext = new ActionExecutingContext();
            actionExecutingContext.ActionParameters = new Dictionary<string, object>();
            userStoreMock = MockRepository.GenerateMock<IUserStore<ApplicationUser>>();
            userManager = new UserManager<ApplicationUser>(userStoreMock);
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
        public void ItThrowsAnInvalidOperationExceptionIfTheUserIsNotAuthenticated()
        {
            identity.BackToRecord(BackToRecordOptions.All);
            identity.Expect(mock => mock.IsAuthenticated)
                .Repeat.Once()
                .Return(false);

            var exception = Assert.Throws<InvalidOperationException>(() => userContextActionFilter.OnActionExecuting(actionExecutingContext, userManager));
            Assert.AreEqual(UserContextAttribute.EXCEPTION_MESSAGE_USER_NOT_AUTHENTICATED, exception.Message);
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
    }
}
