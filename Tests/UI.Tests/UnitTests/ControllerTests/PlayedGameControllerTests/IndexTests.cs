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
            playedGameController.Index();

            playedGameLogicMock.AssertWasCalled(x => x.GetRecentGames(Controllers.PlayedGameController.NUMBER_OF_RECENT_GAMES_TO_DISPLAY));
        }

        [Test]
        public void ItReturnsTheIndexView()
        {
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
            List<PlayedGameDetails> summaries = new List<PlayedGameDetails>();
            recentGamesSummaryBuilder.Expect(builder => builder.Build(playedGames)).Repeat.Once()
                .Return(summaries);
            ViewResult result = playedGameController.Index() as ViewResult;

            PlayedGameDetails viewModel = (PlayedGameDetails)result.ViewData.Model;
            Assert.AreEqual(summaries, viewModel);
        }


        [Test]
        public void ItGeneratesTheViewModel()
        {
            List<BusinessLogic.Models.PlayedGame> recentlyPlayedGames = new List<BusinessLogic.Models.PlayedGame>();
            playedGameLogicMock.Expect(playedGameLogic => playedGameLogic.GetRecentGames(Arg<int>.Is.Anything))
                .Repeat.Once()
                .Return(recentlyPlayedGames);
          
            playedGameController.Index();
            
            recentGamesSummaryBuilder.AssertWasCalled(builder => builder.Build(recentlyPlayedGames));
        }
    }
}
