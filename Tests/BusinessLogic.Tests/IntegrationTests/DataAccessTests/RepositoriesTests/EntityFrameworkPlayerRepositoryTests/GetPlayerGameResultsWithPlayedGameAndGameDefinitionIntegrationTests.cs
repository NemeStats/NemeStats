using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.IntegrationTests.DataAccessTests.RepositoriesTests.EntityFrameworkPlayerRepositoryTests
{
    [TestFixture]
    public class GetPlayerGameResultsWithPlayedGameAndGameDefinitionIntegrationTests : IntegrationTestBase
    {
        private DataContext dataContext;
        private EntityFrameworkPlayerRepository playerRepository;

        [SetUp]
        public void TestSetUp()
        {
            dataContext = new NemeStatsDataContext();
            playerRepository = new EntityFrameworkPlayerRepository(dataContext);
        }

        //TODO need more integration tests

        [Test]
        public void ItReturnsAnEmptyListIfThePlayersGamingGroupDoesntMatchTheCurrentUser()
        {
            List<PlayerGameResult> playerGameResults = playerRepository
                .GetPlayerGameResultsWithPlayedGameAndGameDefinition(testPlayer1.Id, 1, testUserWithOtherGamingGroup);

            Assert.AreEqual(0, playerGameResults.Count);
        }

        [TearDown]
        public void TearDown()
        {
            dataContext.Dispose();
        }
    }
}
