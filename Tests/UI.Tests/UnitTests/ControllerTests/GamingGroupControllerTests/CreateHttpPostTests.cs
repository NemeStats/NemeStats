using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class CreateHttpPostTests : GamingGroupControllerTestBase
    {
        private string gamingGroupName = "name";

        [Test]
        public void ItRedirectsToTheGamingGroupIndexView()
        {
            RedirectToRouteResult result = gamingGroupController.Create(gamingGroupName, userContext) as RedirectToRouteResult;

            Assert.AreEqual(MVC.GamingGroup.ActionNames.Index, result.RouteValues["action"]);
        }

        [Test]
        public void ItCreatesANewGamingGroup()
        {
            string gamingGroupName = "name";

            gamingGroupController.Create(gamingGroupName, userContext);

            gamingGroupCreator.AssertWasCalled(mock => mock.CreateGamingGroupAsync(gamingGroupName, userContext));
        }
    }
}
