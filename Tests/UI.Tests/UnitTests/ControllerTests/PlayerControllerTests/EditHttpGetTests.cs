using BusinessLogic.Models.Players;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.PlayerControllerTests
{
    [TestFixture]
    public class EditHttpGetTests : PlayerControllerTestBase
    {
        [Test]
        public void ItReturnsABadRequestHttpStatusCodeIfThereIsNoPlayerId()
        {
            int? nullPlayerId = null;
            HttpStatusCodeResult result = playerController.Edit(nullPlayerId) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public void ItReturnsAnUnauthorizedAccessHttpStatusCodeIfTheUserDoesntHaveAccess()
        {
            int playerId = 1;
            playerRetrieverMock.Expect(mock => mock.GetPlayerDetails(playerId, 0))
                .Throw(new UnauthorizedAccessException());
            HttpStatusCodeResult result = playerController.Edit(playerId) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Test]
        public void ItReturnsANotFoundHttpStatusCodeIfThePlayerDoesntExist()
        {
            int playerId = -1;
            playerRetrieverMock.Expect(mock => mock.GetPlayerDetails(playerId, 0))
                .Throw(new KeyNotFoundException());
            HttpStatusCodeResult result = playerController.Edit(playerId) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Test]
        public void ItLoadsTheEditView()
        {
            PlayerDetails playerDetails = new PlayerDetails();

            ViewResult result = playerController.Edit(playerDetails.Id) as ViewResult;

            Assert.AreEqual(MVC.Player.Views.Edit, result.ViewName);
        }

        [Test]
        public void ItPutsThePlayerDetailsOnTheView()
        {
            PlayerDetails playerDetails = new PlayerDetails();
            playerRetrieverMock.Expect(mock => mock.GetPlayerDetails(playerDetails.Id, 0))
                .Repeat.Once()
                .Return(playerDetails);

            ViewResult result = playerController.Edit(playerDetails.Id) as ViewResult;

            Assert.AreSame(playerDetails, result.Model);
        }
    }
}
