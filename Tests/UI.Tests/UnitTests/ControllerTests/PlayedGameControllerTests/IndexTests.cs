using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Web.Mvc;
using UI.Models.PlayedGame;
using UI.Transformations;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
    [TestFixture]
    public class IndexTests : TestBase
    {
        [Test]
        public void ItGetsRecentlyPlayedGames()
        {
            playedGameLogicMock.Expect(x => x.GetRecentGames(Controllers.PlayedGameController.NUMBER_OF_RECENT_GAMES_TO_DISPLAY))
                .Repeat.Once()
                .Return(new List<PlayedGame>());
            playedGameController.Index();

            playedGameLogicMock.AssertWasCalled(x => x.GetRecentGames(Controllers.PlayedGameController.NUMBER_OF_RECENT_GAMES_TO_DISPLAY));
        }

        [Test]
        public void ItReturnsTheIndexView()
        {
            playedGameLogicMock.Expect(x => x.GetRecentGames(Controllers.PlayedGameController.NUMBER_OF_RECENT_GAMES_TO_DISPLAY))
                .Repeat.Once()
                .Return(new List<PlayedGame>());
            ViewResult result = playedGameController.Index() as ViewResult;

            Assert.AreEqual(MVC.PlayedGame.Views.Index, result.ViewName);
        }

        [Test]
        public void TheIndexViewHasTheGamesSet()
        {
            int playedGameId = 13541;

            List<PlayedGame> playedGames = new List<PlayedGame>() { new PlayedGame() { Id = playedGameId } };
            playedGameLogicMock.Expect(x => x.GetRecentGames(Controllers.PlayedGameController.NUMBER_OF_RECENT_GAMES_TO_DISPLAY))
                .Repeat.Once()
                .Return(playedGames);
            List<PlayedGameDetailsViewModel> summaries = new List<PlayedGameDetailsViewModel>()
            {
                new PlayedGameDetailsViewModel() { PlayedGameId = playedGameId }
            };
            playedGameDetailsBuilder.Expect(builder => builder.Build(playedGames[0])).Repeat.Once()
                .Return(summaries[0]);
            ViewResult result = playedGameController.Index() as ViewResult;

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
            playedGameLogicMock.Expect(playedGameLogic => playedGameLogic.GetRecentGames(Arg<int>.Is.Anything))
                .Repeat.Once()
                .Return(recentlyPlayedGames);
          
            playedGameController.Index();
            
            foreach(var playedgame in recentlyPlayedGames)
            {
                playedGameDetailsBuilder.AssertWasCalled(builder => builder.Build(playedgame));
            };
        }
    }
}
