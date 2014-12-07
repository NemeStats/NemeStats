using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using UI.Controllers;
using UI.Models.PlayedGame;
using UI.Models.Players;

namespace UI.Tests.UnitTests.ControllerTests.PlayerControllerTests
{
    [TestFixture]
    public class PlayerControllerTests : PlayerControllerTestBase
    {
        private int playerId = 1;
        private string expectedMinionUrl;

        public override void SetUp()
        {
            base.SetUp();

            expectedMinionUrl = playerController.Url.Action(MVC.Player.ActionNames.Details, MVC.Player.Name, new { id = playerId }, "HTTPS") + "#minions";
        }

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
            playerController.Details(playerId, currentUser);

            playerRetrieverMock.AssertWasCalled(x => x.GetPlayerDetails(playerId, PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE));
        }

        [Test]
        public void ItReturnsThePlayerDetailsViewWhenThePlayerIsFound()
        {
            PlayerDetails playerDetails = new PlayerDetails(){ PlayerGameResults = new List<PlayerGameResult>() };
            playerRetrieverMock.Expect(playerLogic => playerLogic.GetPlayerDetails(playerId, PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE))
                .Repeat.Once()
                .Return(playerDetails);

            PlayerDetailsViewModel playerDetailsViewModel = new PlayerDetailsViewModel()
            {
                PlayerId = playerId,
                PlayerGameResultDetails = new List<GameResultViewModel>()
            };
            playerDetailsViewModelBuilderMock.Expect(viewModelBuilder => viewModelBuilder.Build(playerDetails, expectedMinionUrl, currentUser))
                .Repeat
                .Once()
                .Return(playerDetailsViewModel);
            ViewResult viewResult = playerController.Details(playerId, currentUser) as ViewResult;

            Assert.AreEqual(MVC.Player.Views.Details, viewResult.ViewName);
        }

        [Test]
        public void ItSetsThePlayerDetailsViewModelForTheFoundPlayer()
        {
            PlayerDetails playerDetails = new PlayerDetails() { Id = playerId, PlayerGameResults = new List<PlayerGameResult>() };
            playerRetrieverMock.Expect(playerLogic => playerLogic.GetPlayerDetails(playerId, PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE))
                .Repeat.Once()
                .Return(playerDetails);

            PlayerDetailsViewModel playerDetailsViewModel = new PlayerDetailsViewModel()
            {
                PlayerId = playerId,
                PlayerGameResultDetails = new List<GameResultViewModel>()
            };
            playerDetailsViewModelBuilderMock.Expect(viewModelBuilder => viewModelBuilder.Build(playerDetails, expectedMinionUrl, currentUser))
                .Repeat
                .Once()
                .Return(playerDetailsViewModel);

            ViewResult viewResult = playerController.Details(playerId, currentUser) as ViewResult;

            Assert.AreEqual(playerDetailsViewModel, viewResult.Model);
        }

        [Test]
        public void ItOnlyRetrievesTheSpecifiedNumberOfPlayers()
        {
            playerController.Details(playerId, currentUser);

            playerRetrieverMock.AssertWasCalled(mock => mock.GetPlayerDetails(playerId, PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE));
        }

        [Test]
        public void ItPutsTheRecentGamesMessageOnTheViewbag()
        {
            PlayerDetails playerDetails = new PlayerDetails(){ PlayerGameResults = new List<PlayerGameResult>() };
            playerRetrieverMock.Expect(playerLogic => playerLogic.GetPlayerDetails(
                playerId, 
                PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE))
                .Repeat.Once()
                .Return(playerDetails);

            PlayerDetailsViewModel playerDetailsViewModel = new PlayerDetailsViewModel()
            {
                PlayerId = playerId,
                PlayerGameResultDetails = new List<GameResultViewModel>()
            };
            playerDetailsViewModelBuilderMock.Expect(viewModelBuilder => viewModelBuilder.Build(playerDetails, expectedMinionUrl, currentUser))
                .Repeat
                .Once()
                .Return(playerDetailsViewModel);
            string expectedMessage = "expected message";
            showingXResultsMessageBuilderMock.Expect(mock => mock.BuildMessage(
                PlayerController.NUMBER_OF_RECENT_GAMES_TO_RETRIEVE,
                playerDetailsViewModel.PlayerGameResultDetails.Count))
                    .Return(expectedMessage);

            playerController.Details(playerId, currentUser);

            Assert.AreEqual(expectedMessage, playerController.ViewBag.RecentGamesMessage);
        }
    }
}
