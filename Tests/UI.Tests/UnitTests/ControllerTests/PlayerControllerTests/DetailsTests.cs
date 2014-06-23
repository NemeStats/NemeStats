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
using UI.Models.PlayedGame;
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
        private GameResultViewModelBuilder playerGameResultDetailsBuilder;
        private PlayerDetailsViewModelBuilder playerDetailsViewModelBuilder;
        private PlayerController playerController;

        [SetUp]
        public void SetUp()
        {
            dbContextMock = MockRepository.GenerateMock<NemeStatsDbContext>();
            playerLogicMock = MockRepository.GenerateMock<PlayerLogic>();
            playerGameResultDetailsBuilder = MockRepository.GenerateMock<GameResultViewModelBuilder>();
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

            playerLogicMock.AssertWasCalled(x => x.GetPlayerDetails(playerId, PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE));
        }

        [Test]
        public void ItReturnsThePlayerDetailsViewWhenThePlayerIsFound()
        {
            int playerId = 1351;
            PlayerDetails playerDetails = new PlayerDetails(){ PlayerGameResults = new List<PlayerGameResult>() };
            playerLogicMock.Expect(playerLogic => playerLogic.GetPlayerDetails(playerId, PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE))
                .Repeat.Once()
                .Return(playerDetails);

            PlayerDetailsViewModel playerDetailsViewModel = new PlayerDetailsViewModel()
            {
                PlayerId = playerId,
                PlayerGameResultDetails = new List<GameResultViewModel>()
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
            playerLogicMock.Expect(playerLogic => playerLogic.GetPlayerDetails(playerId, PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE))
                .Repeat.Once()
                .Return(playerDetails);

            PlayerDetailsViewModel playerDetailsViewModel = new PlayerDetailsViewModel()
            {
                PlayerId = playerId,
                PlayerGameResultDetails = new List<GameResultViewModel>()
            };
            playerDetailsViewModelBuilder.Expect(viewModelBuilder => viewModelBuilder.Build(playerDetails))
                .Repeat
                .Once()
                .Return(playerDetailsViewModel);

            ViewResult viewResult = playerController.Details(playerId) as ViewResult;

            Assert.AreEqual(playerDetailsViewModel, viewResult.Model);
        }

        [Test]
        public void ItOnlyRetrievesTheSpecifiedNumberOfPlayers()
        {
            int playerId = 1;

            playerController.Details(playerId);

            playerLogicMock.AssertWasCalled(mock => mock.GetPlayerDetails(playerId, PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE));
        }

        [Test]
        public void ItShowsMessageStatingThatOnlyALimitedListOfRecentGamesAreShowingIfAtMaxGames()
        {
            List<PlayerGameResult> playerGameResults = new List<PlayerGameResult>();
            for(int i = 0; i < PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE; i++)
            {
                playerGameResults.Add(new PlayerGameResult());
            }
            PlayerDetails details = new PlayerDetails(){
                PlayerGameResults = playerGameResults
            };
            int playerId = 1;

            playerLogicMock.Expect(mock => mock.GetPlayerDetails(playerId, PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE))
                .Repeat
                .Once()
                .Return(details);

            playerController.Details(playerId);

            string expectedMessage = string.Format(PlayerController.RECENT_GAMES_MESSAGE_FORMAT,
                PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE);
            Assert.AreEqual(expectedMessage, playerController.ViewBag.RecentGamesMessage);
        }

        [Test]
        public void ItDoesntShowTheRecentGamesMessageIfThereAreLessThanTheRequestedMaxNumberOfGames()
        {
            PlayerDetails playerDetails = new PlayerDetails() { PlayerGameResults = new List<PlayerGameResult>() };
            int playerId = 1;
            playerLogicMock.Expect(mock => mock.GetPlayerDetails(playerId, PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE))
                .Repeat
                .Once()
                .Return(playerDetails);

            playerController.Details(1);

            Assert.IsNull(playerController.ViewBag.RecentGamesMessage);
        }
    }
}
