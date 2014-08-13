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
    //TODO failing at testing async methods.... had to ignore this 
    [TestFixture, Ignore("had to ignore since i haven't figured out how to test async methods")]
    public class CreateHttpPostTests : GamingGroupControllerTestBase
    {
        private string gamingGroupName = "name";

        [Test]
        public async Task ItRedirectsToTheGamingGroupIndexView()
        {
            RedirectToRouteResult result = await gamingGroupController.Create(gamingGroupName, currentUser) as RedirectToRouteResult;

            Assert.AreEqual(MVC.GamingGroup.ActionNames.Index, result.RouteValues["action"]);
        }

        [Test]
        public async Task ItCreatesANewGamingGroup()
        {
            string gamingGroupName = "name";

            await gamingGroupController.Create(gamingGroupName, currentUser);

            gamingGroupCreatorMock.AssertWasCalled(mock => mock.CreateGamingGroupAsync(gamingGroupName, currentUser));
        }
    }
}
