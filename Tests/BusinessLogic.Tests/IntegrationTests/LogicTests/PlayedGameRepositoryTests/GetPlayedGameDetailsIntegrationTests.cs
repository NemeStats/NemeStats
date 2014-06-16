using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.PlayedGameRepositoryTests
{
    [TestFixture]
    public class PlayedGameRepositoryIntegrationTests : IntegrationTestBase
    {
        private PlayedGame playedGame;

        [SetUp]
        public void SetUp()
        {
            playedGame = new BusinessLogic.Models.PlayedGameRepository(dbContext).GetPlayedGameDetails(testPlayedGames[0].Id);
        }

        [Test]
        public void ItRetrievesThePlayedGame()
        {
            Assert.NotNull(playedGame);
        }

        [Test]
        public void ItRetrievesTheGameResults()
        {
            Assert.GreaterOrEqual(testPlayedGames[0].PlayerGameResults.Count, playedGame.PlayerGameResults.Count());
        }

        [Test]
        public void ItRetrievesTheGameDefinition()
        {
            Assert.NotNull(playedGame.GameDefinition);
        }

        [Test]
        public void ItReturnsNullIfNoPlayedGameFound()
        {
            PlayedGame notFoundPlayedGame = new BusinessLogic.Models.PlayedGameRepository(dbContext).GetPlayedGameDetails(-1);
            Assert.Null(notFoundPlayedGame);
        }
    }
}
