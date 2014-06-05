using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Tests.IntegrationTests.Controller.Transformations.PlayedGameTransformations
{
    [TestFixture]
    public class MakeNewlyCompletedGameIntegrationTests
    {
        //TODO Check with Grant
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ItThrowsAnArgumentExceptionIfPlayerGameResultsIsNull()
        {
            UI.Controllers.Transformations.PlayedGameTransformations.MakeNewlyCompletedGame(0, null);
        }

        [Test]
        public void ItSetsTheGameDefinitionId()
        {
            int gameDefinitionId = 1;
            NewlyCompletedGame game = UI.Controllers.Transformations.PlayedGameTransformations.MakeNewlyCompletedGame(gameDefinitionId, new List<PlayerGameResult>());

            Assert.AreEqual(gameDefinitionId, game.GameDefinitionId);
        }

        [Test]
        public void ItSetsThePlayerIdOnEachGameRank()
        {
            int playerOneId = 1;
            int playerTwoId = 2;

            List<PlayerGameResult> playerGameResults = new List<PlayerGameResult>()
            {
                new PlayerGameResult() { PlayerId = playerOneId },
                new PlayerGameResult() { PlayerId = playerTwoId }
            };

            NewlyCompletedGame game = UI.Controllers.Transformations.PlayedGameTransformations.MakeNewlyCompletedGame(0, playerGameResults);

            Assert.NotNull(game.PlayerRanks.Find(x => x.PlayerId == playerOneId));
            Assert.NotNull(game.PlayerRanks.Find(x => x.PlayerId == playerTwoId));
        }

        [Test]
        public void ItSetsTheRankOnEachGameRank()
        {
            int playerOneId = 1;
            int playerTwoId = 2;
            int playerOneRank = 2;
            int playerTwoRank = 1;

            List<PlayerGameResult> playerGameResults = new List<PlayerGameResult>()
            {
                new PlayerGameResult() { PlayerId = playerOneId, GameRank = playerOneRank },
                new PlayerGameResult() { PlayerId = playerTwoId, GameRank = playerTwoRank }
            };

            NewlyCompletedGame game = UI.Controllers.Transformations.PlayedGameTransformations.MakeNewlyCompletedGame(0, playerGameResults);

            Assert.AreEqual(playerOneRank, game.PlayerRanks.Find(x => x.PlayerId == playerOneId).GameRank);
            Assert.AreEqual(playerTwoRank, game.PlayerRanks.Find(x => x.PlayerId == playerTwoId).GameRank);
        }
    }
}
