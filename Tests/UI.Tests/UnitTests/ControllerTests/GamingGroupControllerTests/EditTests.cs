using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class EditTests : GamingGroupControllerTestBase
    {
        [Test]
        public void ItUpdatesTheGamingGroupName()
        {
            string gamingGroupName = "new gaming group name";

            HttpStatusCodeResult httpStatusCodeResult = gamingGroupController.Edit(gamingGroupName, currentUser) as HttpStatusCodeResult;

            gamingGroupSaverMock.AssertWasCalled(saver => saver.UpdateGamingGroupName(gamingGroupName, currentUser));
        }

        [Test]
        public void ItReturnsAJsonResultWhenEverythingIsOk()
        {
            JsonResult jsonResult = gamingGroupController.Edit("gaming group name", currentUser) as JsonResult;

            dynamic jsonData = jsonResult.Data;
            Assert.AreEqual((int)HttpStatusCode.OK, jsonData.StatusCode);
        }
    }
}
