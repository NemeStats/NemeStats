using BusinessLogic.DataAccess;
using BusinessLogic.Logic;
using BusinessLogic.Models;
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

namespace UI.Tests.UnitTests.Controller
{
    [TestFixture]
    public class PlayerControllerTests
    {
        private NerdScorekeeperDbContext dbContextMock;
        private PlayerLogic playerLogicMock;
        private PlayerController playerController;

        [TestFixtureSetUp]
        public void SetUp()
        {
            dbContextMock = MockRepository.GenerateMock<NerdScorekeeperDbContext>();
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
                .Return(new Player());
            ViewResult playerDetails = playerController.Details(playerId) as ViewResult;

            Assert.AreEqual(MVC.Player.Views.Details, playerDetails.ViewName);
        }

        [Test]
        public void ItReturnsThePlayerDetailsViewForTheFoundPlayer()
        {
            int playerId = 1351;
            Player player = new Player() { Id = playerId };
            playerLogicMock.Expect(playerLogic => playerLogic.GetPlayerDetails(playerId))
                .Repeat.Once()
                .Return(player);
            ViewResult playerDetails = playerController.Details(playerId) as ViewResult;

            Assert.AreEqual(player, playerDetails.Model);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            dbContextMock.Dispose();
        }
  
    }
}
