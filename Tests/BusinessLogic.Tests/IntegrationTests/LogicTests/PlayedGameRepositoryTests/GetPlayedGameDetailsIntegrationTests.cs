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
    public class GetPlayedGameDetailsIntegrationTests : IntegrationTestBase
    {
        private PlayedGame GetTestSubjectPlayedGame(NemeStatsDbContext dbContextToTestWith)
        {
            return new BusinessLogic.Models.PlayedGameRepository(dbContextToTestWith)
                .GetPlayedGameDetails(testPlayedGames[0].Id);
        }

        [Test]
        public void ItRetrievesThePlayedGame()
        {
            PlayedGame playedGame = GetTestSubjectPlayedGame(dbContext);
            Assert.NotNull(playedGame);
        }

        [Test]
        public void ItEagerlyFetchesTheGameResults()
        {
            PlayedGame playedGame = GetTestSubjectPlayedGame(dbContextWithLazyLoadingDisabled);
            Assert.GreaterOrEqual(testPlayedGames[0].PlayerGameResults.Count, playedGame.PlayerGameResults.Count());
        }

        [Test]
        public void ItEagerlyFetchesTheGameDefinition()
        {
            PlayedGame playedGame = GetTestSubjectPlayedGame(dbContextWithLazyLoadingDisabled);
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
