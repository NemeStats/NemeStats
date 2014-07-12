using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.PlayerRepositoryTests
{
    [TestFixture]
    public class GetPlayerGameResultsWithPlayedGameAndGameDefinitionIntegrationTests : IntegrationTestBase
    {
        private NemeStatsDbContext dbContext;
        private EntityFrameworkPlayerRepository playerRepository;

        [SetUp]
        public void TestSetUp()
        {
            dbContext = new NemeStatsDbContext();
            playerRepository = new EntityFrameworkPlayerRepository(dbContext);
        }

        //TODO need more integration tests

        [Test]
        public void ItReturnsAnEmptyListIfThePlayersGamingGroupDoesntMatchTheCurrentUser()
        {
            List<PlayerGameResult> playerGameResults = playerRepository
                .GetPlayerGameResultsWithPlayedGameAndGameDefinition(testPlayer1.Id, 1, testUserContextForUserWithOtherGamingGroup);

            Assert.AreEqual(0, playerGameResults.Count);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }
    }
}
