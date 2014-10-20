using BusinessLogic.Logic.GamingGroups;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using UI.Controllers;

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
        public void ItReturnsAnHttpStatusCodeOk()
        {
            HttpStatusCodeResult httpStatusCodeResult = gamingGroupController.Edit("gaming group name", currentUser) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.OK, httpStatusCodeResult.StatusCode);
        }
    }
}
