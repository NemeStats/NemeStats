using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using UI.Controllers;
using UI.Models;
using UI.Models.Players;
using UI.Transformations;
using UI.Transformations.Player;

namespace UI.Tests.UnitTests.ControllerTests.PlayerControllerTests
{
    [TestFixture]
    public class PlayerControllerTests
    {
        private NemeStatsDbContext dbContextMock;
        private PlayerLogic playerLogicMock;
        private PlayerGameResultDetailsViewModelBuilder playerGameResultDetailsBuilder;
        private PlayerDetailsViewModelBuilder playerDetailsViewModelBuilder;
        private PlayerController playerController;

        [TestFixtureSetUp]
        public void SetUp()
        {
            dbContextMock = MockRepository.GenerateMock<NemeStatsDbContext>();
            playerLogicMock = MockRepository.GenerateMock<PlayerLogic>();
            playerGameResultDetailsBuilder = MockRepository.GenerateMock<PlayerGameResultDetailsViewModelBuilder>();
            playerDetailsViewModelBuilder = MockRepository.GenerateMock<PlayerDetailsViewModelBuilder>();
            playerController = new PlayerController(
                                dbContextMock, 
                                playerLogicMock, 
                                playerGameResultDetailsBuilder, 
                                playerDetailsViewModelBuilder);
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
            PlayerDetails playerDetails = new PlayerDetails(){ PlayerGameResults = new List<PlayerGameResult>() };
            playerLogicMock.Expect(playerLogic => playerLogic.GetPlayerDetails(playerId))
                .Repeat.Once()
                .Return(playerDetails);

            PlayerDetailsViewModel playerDetailsViewModel = new PlayerDetailsViewModel()
            {
                PlayerId = playerId,
                PlayerGameSummaries = new List<Models.PlayedGame.IndividualPlayerGameSummaryViewModel>()
            };
            playerDetailsViewModelBuilder.Expect(viewModelBuilder => viewModelBuilder.Build(playerDetails))
                .Repeat
                .Once()
                .Return(playerDetailsViewModel);
            ViewResult viewResult = playerController.Details(playerId) as ViewResult;

            Assert.AreEqual(MVC.Player.Views.Details, viewResult.ViewName);
        }

        [Test]
        public void ItSetsThePlayerDetailsViewModelForTheFoundPlayer()
        {
            int playerId = 1351;
            PlayerDetails playerDetails = new PlayerDetails() { Id = playerId, PlayerGameResults = new List<PlayerGameResult>() };
            playerLogicMock.Expect(playerLogic => playerLogic.GetPlayerDetails(playerId))
                .Repeat.Once()
                .Return(playerDetails);

            PlayerDetailsViewModel playerDetailsViewModel = new PlayerDetailsViewModel()
            {
                PlayerId = playerId,
                PlayerGameSummaries = new List<Models.PlayedGame.IndividualPlayerGameSummaryViewModel>()
            };
            playerDetailsViewModelBuilder.Expect(viewModelBuilder => viewModelBuilder.Build(playerDetails))
                .Repeat
                .Once()
                .Return(playerDetailsViewModel);

            ViewResult viewResult = playerController.Details(playerId) as ViewResult;

            Assert.AreEqual(playerDetailsViewModel, viewResult.Model);
        }
    }
}
