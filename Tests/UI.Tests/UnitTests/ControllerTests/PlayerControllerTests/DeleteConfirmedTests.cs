using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.PlayerControllerTests
{
    [TestFixture]
    public class DeleteConfirmedTests : PlayerControllerTestBase
    {
        [Test]
        public void ItDeletesThePlayer()
        {
            int playerId = 1;
            playerController.DeleteConfirmed(playerId, currentUser);

            playerRepositoryMock.AssertWasCalled(mock => mock.Delete(playerId, currentUser));
        }

        [Test]
        public void ItReturnsAnUnauthorizedAccessHttpStatusIfTheUserDoesntHaveAccess()
        {
            int playerId = 1351;
            playerRepositoryMock.Expect(mock => mock.Delete(playerId, currentUser))
                .Throw(new UnauthorizedAccessException());
            HttpStatusCodeResult result = playerController.DeleteConfirmed(playerId, currentUser) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Test]
        public void ItRedirectsToTheIndexAction()
        {
            int playerId = 1;
            playerRepositoryMock.Expect(mock => mock.Delete(playerId, currentUser));
            RedirectToRouteResult result = playerController.DeleteConfirmed(playerId, currentUser) as RedirectToRouteResult;

            Assert.AreEqual(MVC.Player.ActionNames.Index, result.RouteValues["action"]);
        }
    }
}
