using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
    [TestFixture]
    public class IndexTests : TestBase
    {
        [Test]
        public void ItGetsTheLastTenPlayedGames()
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
            playedGameLogicMock.Expect(x => x.GetRecentGames(Controllers.PlayedGameController.NUMBER_OF_RECENT_GAMES_TO_DISPLAY)).Repeat.Once().Return(playedGames);
            ViewResult result = playedGameController.Index() as ViewResult;

            List<PlayedGame> playedGamesOnViewModel = (List<PlayedGame>)result.ViewData.Model;
            Assert.AreEqual(playedGameId, playedGamesOnViewModel[0].Id);
        }
    }
}
