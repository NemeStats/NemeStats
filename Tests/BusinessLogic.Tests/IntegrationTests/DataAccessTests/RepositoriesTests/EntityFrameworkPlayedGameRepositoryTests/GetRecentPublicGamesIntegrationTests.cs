using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models.Games;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.IntegrationTests.DataAccessTests.RepositoriesTests.EntityFrameworkPlayedGameRepositoryTests
{
    [TestFixture]
    public class GetRecentPublicGamesIntegrationTests : IntegrationTestBase
    {
        private int numberOfGamesToRetrieve = 3;

        List<PublicGameSummary> publicGameSummaryResults;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            using (NemeStatsDataContext dataContext = new NemeStatsDataContext())
            {
                PlayedGameRepository playedGameLogic = new EntityFrameworkPlayedGameRepository(dataContext);

                publicGameSummaryResults = playedGameLogic.GetRecentPublicGames(numberOfGamesToRetrieve);
            }
        }

        [Test]
        public void ItReturnsTheSpecifiedNumberOfGames()
        {
            Assert.True(publicGameSummaryResults.Count == numberOfGamesToRetrieve);
        }

        [Test]
        public void ItReturnsTheGamesOrderedByDatePlayedDescending()
        {
            DateTime lastPlayedDateTime = new DateTime(2099, 1, 1);

            foreach(PublicGameSummary summary in publicGameSummaryResults)
            {
                Assert.GreaterOrEqual(lastPlayedDateTime, summary.DatePlayed);
                lastPlayedDateTime = summary.DatePlayed;
            }
            Assert.True(publicGameSummaryResults.Count == numberOfGamesToRetrieve);
        }
    }
}
