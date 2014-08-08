using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using NUnit.Framework;

namespace BusinessLogic.Tests.IntegrationTests.DataAccessTests.RepositoriesTests.PlayedGameRepositoryTests
{
    [TestFixture]
    public class CreatePlayedGameIntegrationTests : IntegrationTestBase
    {
        [Test]
        public void ItCreatesATwoPlayerPlayedGameAndSetsTheNumberOfPlayers()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                PlayedGame playedGameFromTheDatabase = dbContext.PlayedGames.Find(testPlayedGames[0].Id);

                Assert.IsTrue(playedGameFromTheDatabase.NumberOfPlayers == 2);
            }
        }

        //TODO need more integration tests, but have been looking at the database manually and it looks OK.
    }
}
