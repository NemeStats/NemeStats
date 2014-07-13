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
    public class DeleteTests : PlayerControllerTestBase
    {
        [Test]
        public void ItReturnsABadRequestHttpStatusIfNoPlayerIdIsPassed()
        {
            int? nullPlayerId = null;

            HttpStatusCodeResult result = playerController.Delete(nullPlayerId, userContext) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public void ItReturnsAnUnauthorizedAccessHttpStatusIfTheUserDoesntHaveAccess()
        {
            int playerId = 1351;
            playerRepositoryMock.Expect(mock => mock.GetPlayerDetails(playerId, 0, userContext))
                .Throw(new UnauthorizedAccessException());
            HttpStatusCodeResult result = playerController.Delete(playerId, userContext) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Test]
        public void ItReturnsANotFoundHttpStatusIfThePlayerDoesntExist()
        {
            int playerId = -1;
            playerRepositoryMock.Expect(mock => mock.GetPlayerDetails(playerId, 0, userContext))
                .Throw(new KeyNotFoundException());
            HttpStatusCodeResult result = playerController.Delete(playerId, userContext) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Test]
        public void ItShowsTheDeleteView()
        {
            PlayerDetails player = new PlayerDetails();
            playerRepositoryMock.Expect(mock => mock.GetPlayerDetails(player.Id, 0, userContext))
                .Repeat.Once()
                .Return(player);
            ViewResult result = playerController.Delete(player.Id, userContext) as ViewResult;

            Assert.AreEqual(MVC.Player.Views.Delete, result.ViewName);
        }

        [Test]
        public void ItPutsThePlayerOnTheView()
        {
            PlayerDetails player = new PlayerDetails();
            playerRepositoryMock.Expect(mock => mock.GetPlayerDetails(player.Id, 0, userContext))
                .Repeat.Once()
                .Return(player);
            ViewResult result = playerController.Delete(player.Id, userContext) as ViewResult;

            Assert.AreSame(player, result.Model);
        }
    }
}
