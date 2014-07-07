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
        private UserNameActionFilter userNameActionFilter;
        private IIdentity identity;

        [SetUp]
        public void SetUp()
        {
            actionExecutingContext = new ActionExecutingContext();
            actionExecutingContext.ActionParameters = new Dictionary<string, object>();
            //need to simulate like the parameter exists on the method
            actionExecutingContext.ActionParameters[UserNameActionFilter.UserNameKey] = null;

            HttpContextBase httpContextBase = MockRepository.GenerateMock<HttpContextBase>();
            actionExecutingContext.HttpContext = httpContextBase;

            IPrincipal principal = MockRepository.GenerateMock<IPrincipal>();
            httpContextBase.Expect(contextBase => contextBase.User)
                .Repeat.Twice()
                .Return(principal);

            identity = MockRepository.GenerateMock<IIdentity>();
            principal.Expect(mock => mock.Identity)
                .Repeat.Twice()
                .Return(identity);

            identity.Expect(mock => mock.IsAuthenticated)
                .Repeat.Once()
                .Return(true);
            identity.Expect(mock => mock.Name)
                .Repeat.Once()
                .Return(userName);

            userNameActionFilter = new UserNameActionFilter();
        }

        [Test]
        public void ItThrowsAnInvalidOperationExceptionIfTheUserIsNotAuthenticated()
        {
            identity.BackToRecord(BackToRecordOptions.All);
            identity.Expect(mock => mock.IsAuthenticated)
                .Repeat.Once()
                .Return(false);

            var exception = Assert.Throws<InvalidOperationException>(() => userNameActionFilter.OnActionExecuting(actionExecutingContext));
            Assert.AreEqual(UserNameActionFilter.EXCEPTION_MESSAGE_USER_NOT_AUTHENTICATED, exception.Message);
        }

        [Test]
        public void ItSetsTheUserNameActionParameterIfItIsntAlreadySet()
        {
            userNameActionFilter.OnActionExecuting(actionExecutingContext);

            Assert.AreEqual(userName, actionExecutingContext.ActionParameters[UserNameActionFilter.UserNameKey]);
        }
    }
}
