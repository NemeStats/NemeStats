using BusinessLogic.DataAccess;
using BusinessLogic.Logic;
using BusinessLogic.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//  These tests assume that the DataSeeder has run so that there is some baseline data.
namespace BusinessLogic.Tests.IntegrationTests.Logic
{
    [TestFixture]
    public class GetPlayerDetailsIntegrationTests
    {
        private NemeStatsDbContext dbContext;
        private Player dave;

        [TestFixtureSetUp]
        public void SetUp()
        {
            dbContext = new NemeStatsDbContext();
            int davePlayerId = dbContext.Players.First(x => x.Name == DataSeeder.PLAYER_NAME_DAVE).Id;
            dave = new PlayerRepository(dbContext).GetPlayerDetails(davePlayerId);
        }

        [Test]
        public void ItRetrievesPlayerGameResultsInfo()
        {
            Assert.NotNull(dave.PlayerGameResults, "Failed to retrieve PlayerGameResults.");
        }

        [Test]
        public void DaveHasAtLeastOneTwoPlayedGames()
        {
            Assert.GreaterOrEqual(dave.PlayerGameResults.Count(), 2);
        }

        [Test]
        public void ItRetrievesThePlayedGame()
        {
            Assert.NotNull(dave.PlayerGameResults.First().PlayedGame);
        }

        [Test]
        public void ItRetrievesTheGameDefinition()
        {
            Assert.NotNull(dave.PlayerGameResults.First().PlayedGame.GameDefinition);
        }

        [Test]
        public void ItReturnsNullIfNoPlayerFound()
        {
            Player notFoundPlayer = new PlayerRepository(dbContext).GetPlayerDetails(-1);
            Assert.Null(notFoundPlayer);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

    }
}
