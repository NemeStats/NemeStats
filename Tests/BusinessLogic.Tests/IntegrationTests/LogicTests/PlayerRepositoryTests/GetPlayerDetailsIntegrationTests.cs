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
        private PlayerDetails testPlayerDetails;

        [SetUp]
        public void SetUp()
        {
            PlayerRepository playerRepository = new PlayerRepository(dbContext);
            testPlayerDetails = playerRepository.GetPlayerDetails(testPlayer1.Id);
        }

        [Test]
        public void ItEagerlyFetchesPlayerGameResults()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            dbContext.Configuration.ProxyCreationEnabled = false;

            PlayerRepository playerRepository = new PlayerRepository(dbContext);
            PlayerDetails playerDetails = playerRepository.GetPlayerDetails(testPlayer1.Id);
            Assert.NotNull(playerDetails.PlayerGameResults, "Failed to retrieve PlayerGameResults.");
        }

        [Test]
        public void ItEagerlyFetchesPlayedGames()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            dbContext.Configuration.ProxyCreationEnabled = false;

            Assert.NotNull(testPlayerDetails.PlayerGameResults.First().PlayedGame);
        }

        [Test]
        public void ItEagerlyFetchesGameDefinitions()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            dbContext.Configuration.ProxyCreationEnabled = false;

            Assert.NotNull(testPlayerDetails.PlayerGameResults.First().PlayedGame.GameDefinition);
        }

        [Test]
        public void ItReturnsNullIfNoPlayerFound()
        {
            PlayerDetails notFoundPlayer = new PlayerRepository(dbContext).GetPlayerDetails(-1);
            Assert.Null(notFoundPlayer);
        }

        [Test]
        public void ItSetsPlayerStatistics()
        {
            PlayerDetails playerDetails = new PlayerRepository(dbContext).GetPlayerDetails(testPlayer1.Id);

            Assert.NotNull(playerDetails.PlayerStats);
        }

    }
}
