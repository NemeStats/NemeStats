using BusinessLogic.Models.Players;
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
    public class EditTests : PlayerControllerTestBase
    {
        [Test]
        public void ItReturnsABadRequestHttpStatusCodeIfThereIsNoPlayerId()
        {
            int? nullPlayerId = null;
            HttpStatusCodeResult result = playerController.Edit(nullPlayerId, userContext) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public void ItReturnsAnUnauthorizedAccessHttpStatusCodeIfTheUserDoesntHaveAccess()
        {
            int playerId = 1;
            playerRepositoryMock.Expect(mock => mock.GetPlayerDetails(playerId, 0, userContext))
                .Throw(new UnauthorizedAccessException());
            HttpStatusCodeResult result = playerController.Edit(playerId, userContext) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Test]
        public void ItReturnsANotFoundHttpStatusCodeIfThePlayerDoesntExist()
        {
            int playerId = -1;
            playerRepositoryMock.Expect(mock => mock.GetPlayerDetails(playerId, 0, userContext))
                .Throw(new KeyNotFoundException());
            HttpStatusCodeResult result = playerController.Edit(playerId, userContext) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Test]
        public void ItLoadsTheEditView()
        {
            PlayerDetails playerDetails = new PlayerDetails();

            ViewResult result = playerController.Edit(playerDetails.Id, userContext) as ViewResult;

            Assert.AreEqual(MVC.Player.Views.Edit, result.ViewName);
        }

        [Test]
        public void ItPutsThePlayerDetailsOnTheView()
        {
            PlayerDetails playerDetails = new PlayerDetails();
            playerRepositoryMock.Expect(mock => mock.GetPlayerDetails(playerDetails.Id, 0, userContext))
                .Repeat.Once()
                .Return(playerDetails);

            ViewResult result = playerController.Edit(playerDetails.Id, userContext) as ViewResult;

            Assert.AreSame(playerDetails, result.Model);
        }
    }
}
