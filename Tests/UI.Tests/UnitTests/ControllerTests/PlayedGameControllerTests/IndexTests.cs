using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Web.Mvc;
using UI.Controllers;
using UI.Models.PlayedGame;
using UI.Transformations;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
    [TestFixture]
    public class IndexTests : PlayedGameControllerTestBase
    {
        [Test]
        public void ItGetsRecentlyPlayedGames()
        {
            playedGameRetriever.Expect(x => x.GetRecentGames(
                    Controllers.PlayedGameController.NUMBER_OF_RECENT_GAMES_TO_DISPLAY, 
                    currentUser.CurrentGamingGroupId.Value))
                .Repeat.Once()
                .Return(new List<PlayedGame>());
            playedGameController.Index(currentUser);

            playedGameRetriever.AssertWasCalled(x => x.GetRecentGames(
                Controllers.PlayedGameController.NUMBER_OF_RECENT_GAMES_TO_DISPLAY, 
                currentUser.CurrentGamingGroupId.Value));
        }

        [Test]
        public void ItReturnsTheIndexView()
        {
            playedGameRetriever.Expect(x => x.GetRecentGames(
                    Controllers.PlayedGameController.NUMBER_OF_RECENT_GAMES_TO_DISPLAY, 
                    currentUser.CurrentGamingGroupId.Value))
                .Repeat.Once()
                .Return(new List<PlayedGame>());
            ViewResult result = playedGameController.Index(currentUser) as ViewResult;

            Assert.AreEqual(MVC.PlayedGame.Views.Index, result.ViewName);
        }

        [Test]
        public void TheIndexViewHasTheGamesSet()
        {
            int playedGameId = 13541;

            List<PlayedGame> playedGames = new List<PlayedGame>() { new PlayedGame() { Id = playedGameId } };
            playedGameRetriever.Expect(x => x.GetRecentGames(
                    Controllers.PlayedGameController.NUMBER_OF_RECENT_GAMES_TO_DISPLAY, 
                    currentUser.CurrentGamingGroupId.Value))
                .Repeat.Once()
                .Return(playedGames);
            List<PlayedGameDetailsViewModel> summaries = new List<PlayedGameDetailsViewModel>()
            {
                new PlayedGameDetailsViewModel() { PlayedGameId = playedGameId }
            };
            playedGameDetailsBuilderMock.Expect(builder => builder.Build(playedGames[0], currentUser)).Repeat.Once()
                .Return(summaries[0]);

            ViewResult result = playedGameController.Index(currentUser) as ViewResult;

            List<PlayedGameDetailsViewModel> viewModel = (List<PlayedGameDetailsViewModel>)result.ViewData.Model;
            Assert.AreEqual(summaries, viewModel);
        }


        [Test]
        public void ItGeneratesTheViewModel()
        {
            List<BusinessLogic.Models.PlayedGame> recentlyPlayedGames = new List<BusinessLogic.Models.PlayedGame>()
            {
                new PlayedGame(){ Id = 1 },
                new PlayedGame(){ Id = 2 }
            };
            playedGameRetriever.Expect(playedGameLogic => playedGameLogic.GetRecentGames(
                    Arg<int>.Is.Anything, 
                    Arg<int>.Is.Anything))
                .Repeat.Once()
                .Return(recentlyPlayedGames);

            playedGameController.Index(currentUser);
            
            foreach(var playedgame in recentlyPlayedGames)
            {
                playedGameDetailsBuilderMock.AssertWasCalled(builder => builder.Build(playedgame, currentUser));
            };
        }

        [Test]
        public void ItPutsTheRecentGamesMessageOnTheViewBag()
        {
            string expectedMessage = "expected message";
            List<BusinessLogic.Models.PlayedGame> recentlyPlayedGames = new List<BusinessLogic.Models.PlayedGame>()
            {
                new PlayedGame(){ Id = 1 },
                new PlayedGame(){ Id = 2 }
            };
            playedGameRetriever.Expect(playedGameLogic => playedGameLogic.GetRecentGames(Arg<int>.Is.Anything, Arg<int>.Is.Anything))
                .Repeat.Once()
                .Return(recentlyPlayedGames);
            showingXResultsMessageBuilderMock.Expect(mock => mock.BuildMessage(
                PlayedGameController.NUMBER_OF_RECENT_GAMES_TO_DISPLAY,
                recentlyPlayedGames.Count))
                    .Return(expectedMessage);

            playedGameController.Index(currentUser);

            Assert.AreEqual(expectedMessage, playedGameController.ViewBag.RecentGamesMessage);
        }
    }
}
