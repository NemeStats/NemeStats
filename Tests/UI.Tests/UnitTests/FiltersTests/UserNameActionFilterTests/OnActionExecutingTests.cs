using BusinessLogic.DataAccess;
using BusinessLogic.Logic;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using UI.Filters;

namespace UI.Tests.UnitTests.FiltersTests.UserNameActionFilterTests
{
    [TestFixture]
    public class OnActionExecutingTests
    {
        private string userName = "the user name";
        private ActionExecutingContext actionExecutingContext;
        private UserContextActionFilter userContextActionFilter;
        private IIdentity identity;
        private UserContext userContext;

        [SetUp]
        public void SetUp()
        {
            actionExecutingContext = new ActionExecutingContext();
            actionExecutingContext.ActionParameters = new Dictionary<string, object>();
            //need to simulate like the parameter exists on the method
            actionExecutingContext.ActionParameters[UserContextActionFilter.USER_CONTEXT_KEY] = null;

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

            userContextActionFilter = new UserContextActionFilter();
            userContextActionFilter.dbContext = MockRepository.GenerateMock<NemeStatsDbContext>();

            UserContextBuilder userContextBuilder = MockRepository.GenerateMock<UserContextBuilder>();
            userContext = new UserContext()
            {
                ApplicationUserId = "user id",
                GamingGroupId = 135151
            };
            userContextBuilder.Expect(mock => mock.GetUserContext(Arg<string>.Is.Anything, Arg<NemeStatsDbContext>.Is.Same(userContextActionFilter.dbContext)))
                .Repeat.Once()
                .Return(userContext);

            userContextActionFilter.userContextBuilder = userContextBuilder;
        }

        [Test]
        public void ItThrowsAnInvalidOperationExceptionIfTheUserIsNotAuthenticated()
        {
            identity.BackToRecord(BackToRecordOptions.All);
            identity.Expect(mock => mock.IsAuthenticated)
                .Repeat.Once()
                .Return(false);

            var exception = Assert.Throws<InvalidOperationException>(() => userContextActionFilter.OnActionExecuting(actionExecutingContext));
            Assert.AreEqual(UserContextActionFilter.EXCEPTION_MESSAGE_USER_NOT_AUTHENTICATED, exception.Message);
        }

        [Test]
        public void ItSetsTheUserConextActionParameterIfItIsntAlreadySet()
        {
            userContextActionFilter.OnActionExecuting(actionExecutingContext);

            Assert.AreEqual(userContext, actionExecutingContext.ActionParameters[UserContextActionFilter.USER_CONTEXT_KEY]);
        }
    }
}
