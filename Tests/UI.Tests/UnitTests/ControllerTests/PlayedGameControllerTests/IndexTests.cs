using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Web.Mvc;
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
            playedGameLogicMock.Expect(x => x.GetRecentGames(Controllers.PlayedGameController.NUMBER_OF_RECENT_GAMES_TO_DISPLAY, userContext))
                .Repeat.Once()
                .Return(new List<PlayedGame>());
            playedGameController.Index(userContext);

            playedGameLogicMock.AssertWasCalled(x => x.GetRecentGames(Controllers.PlayedGameController.NUMBER_OF_RECENT_GAMES_TO_DISPLAY, userContext));
        }

        [Test]
        public void ItReturnsTheIndexView()
        {
            playedGameLogicMock.Expect(x => x.GetRecentGames(Controllers.PlayedGameController.NUMBER_OF_RECENT_GAMES_TO_DISPLAY, userContext))
                .Repeat.Once()
                .Return(new List<PlayedGame>());
            ViewResult result = playedGameController.Index(userContext) as ViewResult;

            Assert.AreEqual(MVC.PlayedGame.Views.Index, result.ViewName);
        }

        [Test]
        public void TheIndexViewHasTheGamesSet()
        {
            int playedGameId = 13541;

            List<PlayedGame> playedGames = new List<PlayedGame>() { new PlayedGame() { Id = playedGameId } };
            playedGameLogicMock.Expect(x => x.GetRecentGames(Controllers.PlayedGameController.NUMBER_OF_RECENT_GAMES_TO_DISPLAY, userContext))
                .Repeat.Once()
                .Return(playedGames);
            List<PlayedGameDetailsViewModel> summaries = new List<PlayedGameDetailsViewModel>()
            {
                new PlayedGameDetailsViewModel() { PlayedGameId = playedGameId }
            };
            playedGameDetailsBuilderMock.Expect(builder => builder.Build(playedGames[0])).Repeat.Once()
                .Return(summaries[0]);

            ViewResult result = playedGameController.Index(userContext) as ViewResult;

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
            playedGameLogicMock.Expect(playedGameLogic => playedGameLogic.GetRecentGames(Arg<int>.Is.Anything, Arg<UserContext>.Is.Anything))
                .Repeat.Once()
                .Return(recentlyPlayedGames);

            playedGameController.Index(userContext);
            
            foreach(var playedgame in recentlyPlayedGames)
            {
                playedGameDetailsBuilderMock.AssertWasCalled(builder => builder.Build(playedgame));
            };
        }
    }
}
