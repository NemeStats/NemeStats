using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models;
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
using UI.Controllers;

namespace UI.Tests.UnitTests.ControllerTests.PlayerControllerTests
{
    [TestFixture]
    public class PlayerControllerTests
    {
        private NemeStatsDbContext dbContextMock;
        private PlayerLogic playerLogicMock;
        private PlayerController playerController;

        [TestFixtureSetUp]
        public void SetUp()
        {
            dbContextMock = MockRepository.GenerateMock<NemeStatsDbContext>();
            playerLogicMock = MockRepository.GenerateMock<PlayerLogic>();
            playerController = new PlayerController(dbContextMock, playerLogicMock);
        }

        [Test]
        public void ItNeverReturnsNull()
        {
            Assert.NotNull(playerController.Details(null));
        }

        [Test]
        public void ItReturnsBadHttpStatusWhenNoPlayerIdGiven()
        {
            HttpStatusCodeResult actualResult = playerController.Details(null) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, actualResult.StatusCode);
        }

        [Test]
        public void ItReturns404StatusWhenNoPlayerIsFound()
        {
            HttpStatusCodeResult actualResult = playerController.Details(-1) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.NotFound, actualResult.StatusCode);
        }

        [Test]
        public void ItRetrievesRequestedPlayer()
        {
            int playerId = 1351;
            playerController.Details(playerId);

            playerLogicMock.AssertWasCalled(x => x.GetPlayerDetails(playerId));
        }

        [Test]
        public void ItReturnsThePlayerDetailsViewWhenThePlayerIsFound()
        {
            int playerId = 1351;
            playerLogicMock.Expect(playerLogic => playerLogic.GetPlayerDetails(playerId))
                .Repeat.Once()
                .Return(new PlayerDetails());
            ViewResult playerDetails = playerController.Details(playerId) as ViewResult;

            Assert.AreEqual(MVC.Player.Views.Details, playerDetails.ViewName);
        }

        [Test]
        public void ItReturnsThePlayerDetailsViewForTheFoundPlayer()
        {
            int playerId = 1351;
            PlayerDetails playerDetails = new PlayerDetails() { Id = playerId };
            playerLogicMock.Expect(playerLogic => playerLogic.GetPlayerDetails(playerId))
                .Repeat.Once()
                .Return(playerDetails);
            ViewResult player = playerController.Details(playerId) as ViewResult;

            Assert.AreEqual(playerDetails, player.Model);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            dbContextMock.Dispose();
        }
  
    }
}
