using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class GrantAccessTests : GamingGroupControllerTestBase
    {
        [Test]
        public void ItRedirectsToTheIndexAction()
        {
            RedirectToRouteResult redirectResult = gamingGroupController.GrantAccess(string.Empty, userContext) as RedirectToRouteResult;

            Assert.AreEqual(MVC.GamingGroup.ActionNames.Index, redirectResult.RouteValues["action"]);
        }
    }
}
