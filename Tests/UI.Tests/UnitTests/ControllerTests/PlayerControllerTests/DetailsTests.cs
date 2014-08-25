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
            playerController.Details(playerId, currentUser);

            playerRepositoryMock.AssertWasCalled(x => x.GetPlayerDetails(playerId, PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE, currentUser));
        }

        [Test]
        public void ItReturnsThePlayerDetailsViewWhenThePlayerIsFound()
        {
            int playerId = 1351;
            PlayerDetails playerDetails = new PlayerDetails(){ PlayerGameResults = new List<PlayerGameResult>() };
            playerRepositoryMock.Expect(playerLogic => playerLogic.GetPlayerDetails(playerId, PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE, currentUser))
                .Repeat.Once()
                .Return(playerDetails);

            PlayerDetailsViewModel playerDetailsViewModel = new PlayerDetailsViewModel()
            {
                PlayerId = playerId,
                PlayerGameResultDetails = new List<GameResultViewModel>()
            };
            playerDetailsViewModelBuilderMock.Expect(viewModelBuilder => viewModelBuilder.Build(playerDetails, currentUser))
                .Repeat
                .Once()
                .Return(playerDetailsViewModel);
            ViewResult viewResult = playerController.Details(playerId, currentUser) as ViewResult;

            Assert.AreEqual(MVC.Player.Views.Details, viewResult.ViewName);
        }

        [Test]
        public void ItSetsThePlayerDetailsViewModelForTheFoundPlayer()
        {
            int playerId = 1351;
            PlayerDetails playerDetails = new PlayerDetails() { Id = playerId, PlayerGameResults = new List<PlayerGameResult>() };
            playerRepositoryMock.Expect(playerLogic => playerLogic.GetPlayerDetails(playerId, PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE, currentUser))
                .Repeat.Once()
                .Return(playerDetails);

            PlayerDetailsViewModel playerDetailsViewModel = new PlayerDetailsViewModel()
            {
                PlayerId = playerId,
                PlayerGameResultDetails = new List<GameResultViewModel>()
            };
            playerDetailsViewModelBuilderMock.Expect(viewModelBuilder => viewModelBuilder.Build(playerDetails, currentUser))
                .Repeat
                .Once()
                .Return(playerDetailsViewModel);

            ViewResult viewResult = playerController.Details(playerId, currentUser) as ViewResult;

            Assert.AreEqual(playerDetailsViewModel, viewResult.Model);
        }

        [Test]
        public void ItOnlyRetrievesTheSpecifiedNumberOfPlayers()
        {
            int playerId = 1;

            playerController.Details(playerId, currentUser);

            playerRepositoryMock.AssertWasCalled(mock => mock.GetPlayerDetails(playerId, PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE, currentUser));
        }

        [Test]
        public void ItPutsTheRecentGamesMessageOnTheViewbag()
        {
            int playerId = 1;
            PlayerDetails playerDetails = new PlayerDetails(){ PlayerGameResults = new List<PlayerGameResult>() };
            playerRepositoryMock.Expect(playerLogic => playerLogic.GetPlayerDetails(
                playerId, 
                PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE, 
                currentUser))
                .Repeat.Once()
                .Return(playerDetails);

            PlayerDetailsViewModel playerDetailsViewModel = new PlayerDetailsViewModel()
            {
                PlayerId = playerId,
                PlayerGameResultDetails = new List<GameResultViewModel>()
            };
            playerDetailsViewModelBuilderMock.Expect(viewModelBuilder => viewModelBuilder.Build(playerDetails, currentUser))
                .Repeat
                .Once()
                .Return(playerDetailsViewModel);
            string expectedMessage = "expected message";
            showingXResultsMessageBuilder.Expect(mock => mock.BuildMessage(
                PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE,
                playerDetailsViewModel.PlayerGameResultDetails.Count))
                    .Return(expectedMessage);

            playerController.Details(playerId, currentUser);

            Assert.AreEqual(expectedMessage, playerController.ViewBag.RecentGamesMessage);
        }
    }
}
