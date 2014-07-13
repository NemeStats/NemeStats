using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using UI.Controllers;
using UI.Models.PlayedGame;
using UI.Models.Players;
using UI.Transformations;
using UI.Transformations.Player;

namespace UI.Tests.UnitTests.ControllerTests.PlayerControllerTests
{
    [TestFixture]
    public class PlayerControllerTests : PlayerControllerTestBase
    {
        [Test]
        public void ItNeverReturnsNull()
        {
            Assert.NotNull(playerController.Details(null, null));
        }

        [Test]
        public void ItReturnsBadHttpStatusWhenNoPlayerIdGiven()
        {
            HttpStatusCodeResult actualResult = playerController.Details(null, null) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, actualResult.StatusCode);
        }

        [Test]
        public void ItReturns404StatusWhenNoPlayerIsFound()
        {
            HttpStatusCodeResult actualResult = playerController.Details(-1, null) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.NotFound, actualResult.StatusCode);
        }

        [Test]
        public void ItRetrievesRequestedPlayer()
        {
            int playerId = 1351;
            playerController.Details(playerId, userContext);

            playerRepositoryMock.AssertWasCalled(x => x.GetPlayerDetails(playerId, PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE, userContext));
        }

        [Test]
        public void ItReturnsThePlayerDetailsViewWhenThePlayerIsFound()
        {
            int playerId = 1351;
            PlayerDetails playerDetails = new PlayerDetails(){ PlayerGameResults = new List<PlayerGameResult>() };
            playerRepositoryMock.Expect(playerLogic => playerLogic.GetPlayerDetails(playerId, PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE, userContext))
                .Repeat.Once()
                .Return(playerDetails);

            PlayerDetailsViewModel playerDetailsViewModel = new PlayerDetailsViewModel()
            {
                PlayerId = playerId,
                PlayerGameResultDetails = new List<GameResultViewModel>()
            };
            playerDetailsViewModelBuilderMock.Expect(viewModelBuilder => viewModelBuilder.Build(playerDetails))
                .Repeat
                .Once()
                .Return(playerDetailsViewModel);
            ViewResult viewResult = playerController.Details(playerId, userContext) as ViewResult;

            Assert.AreEqual(MVC.Player.Views.Details, viewResult.ViewName);
        }

        [Test]
        public void ItSetsThePlayerDetailsViewModelForTheFoundPlayer()
        {
            int playerId = 1351;
            PlayerDetails playerDetails = new PlayerDetails() { Id = playerId, PlayerGameResults = new List<PlayerGameResult>() };
            playerRepositoryMock.Expect(playerLogic => playerLogic.GetPlayerDetails(playerId, PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE, userContext))
                .Repeat.Once()
                .Return(playerDetails);

            PlayerDetailsViewModel playerDetailsViewModel = new PlayerDetailsViewModel()
            {
                PlayerId = playerId,
                PlayerGameResultDetails = new List<GameResultViewModel>()
            };
            playerDetailsViewModelBuilderMock.Expect(viewModelBuilder => viewModelBuilder.Build(playerDetails))
                .Repeat
                .Once()
                .Return(playerDetailsViewModel);

            ViewResult viewResult = playerController.Details(playerId, userContext) as ViewResult;

            Assert.AreEqual(playerDetailsViewModel, viewResult.Model);
        }

        [Test]
        public void ItOnlyRetrievesTheSpecifiedNumberOfPlayers()
        {
            int playerId = 1;

            playerController.Details(playerId, userContext);

            playerRepositoryMock.AssertWasCalled(mock => mock.GetPlayerDetails(playerId, PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE, userContext));
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

            playerRepositoryMock.Expect(mock => mock.GetPlayerDetails(playerId, PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE, userContext))
                .Repeat
                .Once()
                .Return(details);

            playerController.Details(playerId, userContext);

            string expectedMessage = string.Format(PlayerController.RECENT_GAMES_MESSAGE_FORMAT,
                PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE);
            Assert.AreEqual(expectedMessage, playerController.ViewBag.RecentGamesMessage);
        }

        [Test]
        public void ItDoesntShowTheRecentGamesMessageIfThereAreLessThanTheRequestedMaxNumberOfGames()
        {
            PlayerDetails playerDetails = new PlayerDetails() { PlayerGameResults = new List<PlayerGameResult>() };
            int playerId = 1;
            playerRepositoryMock.Expect(mock => mock.GetPlayerDetails(playerId, PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE, userContext))
                .Repeat
                .Once()
                .Return(playerDetails);

            playerController.Details(1, userContext);

            Assert.IsNull(playerController.ViewBag.RecentGamesMessage);
        }
    }
}
