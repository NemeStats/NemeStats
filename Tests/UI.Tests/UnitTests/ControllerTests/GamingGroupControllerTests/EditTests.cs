using System.Web;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using UI.Controllers.Helpers;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class EditTests : GamingGroupControllerTestBase
    {
        [Test]
        public void ItUpdatesTheGamingGroupName()
        {
            string gamingGroupName = "new gaming group name";

            HttpStatusCodeResult httpStatusCodeResult = gamingGroupControllerPartialMock.Edit(gamingGroupName, currentUser) as HttpStatusCodeResult;

            gamingGroupSaverMock.AssertWasCalled(saver => saver.UpdateGamingGroupName(gamingGroupName, currentUser));
        }

        [Test]
        public void ItReturnsAJsonResultWhenEverythingIsOk()
        {
            JsonResult jsonResult = gamingGroupControllerPartialMock.Edit("gaming group name", currentUser) as JsonResult;

            dynamic jsonData = jsonResult.Data;
            Assert.AreEqual((int)HttpStatusCode.OK, jsonData.StatusCode);
        }

        [Test]
        public void ItClearsTheGamingGroupCookieAfterRenaming()
        {
            gamingGroupControllerPartialMock.Edit("gaming group name", currentUser);

            cookieHelperMock.AssertWasCalled(mock => mock.ClearCookie(
                 Arg<NemeStatsCookieEnum>.Is.Equal(NemeStatsCookieEnum.gamingGroupsCookie),
                 Arg<HttpRequestBase>.Is.Anything,
                 Arg<HttpResponseBase>.Is.Anything));
        }
    }
}
