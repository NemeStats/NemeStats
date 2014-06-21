using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.PlayerRepositoryTests
{
    [TestFixture]
    public class GetPlayerDetailsIntegrationTests : IntegrationTestBase
    {
        [Test]
        public void ItEagerlyFetchesPlayerGameResults()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                PlayerRepository playerRepository = new PlayerRepository(dbContext);

                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Configuration.ProxyCreationEnabled = false;

                PlayerDetails playerDetails = playerRepository.GetPlayerDetails(testPlayer1.Id);
                Assert.NotNull(playerDetails.PlayerGameResults, "Failed to retrieve PlayerGameResults.");
            }
        }

        [Test]
        public void ItEagerlyFetchesPlayedGames()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Configuration.ProxyCreationEnabled = false;
                PlayerRepository playerRepository = new PlayerRepository(dbContext);

                PlayerDetails testPlayerDetails = playerRepository.GetPlayerDetails(testPlayer1.Id);

                Assert.NotNull(testPlayerDetails.PlayerGameResults.First().PlayedGame);
            }
        }

        [Test]
        public void ItEagerlyFetchesGameDefinitions()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Configuration.ProxyCreationEnabled = false;
                PlayerRepository playerRepository = new PlayerRepository(dbContext);

                PlayerDetails testPlayerDetails = playerRepository.GetPlayerDetails(testPlayer1.Id);

                Assert.NotNull(testPlayerDetails.PlayerGameResults.First().PlayedGame.GameDefinition);
            }
        }

        [Test]
        public void ItReturnsNullIfNoPlayerFound()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                PlayerDetails notFoundPlayer = new PlayerRepository(dbContext).GetPlayerDetails(-1);
                Assert.Null(notFoundPlayer);
            }
        }

        [Test]
        public void ItSetsPlayerStatistics()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                PlayerDetails playerDetails = new PlayerRepository(dbContext).GetPlayerDetails(testPlayer1.Id);

                Assert.NotNull(playerDetails.PlayerStats);
            }
        }

        //TODO add test to ensure it orders by date descending
    }
}
