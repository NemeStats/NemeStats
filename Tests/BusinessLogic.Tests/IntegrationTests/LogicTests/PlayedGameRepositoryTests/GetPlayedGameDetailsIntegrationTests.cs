using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic;
using BusinessLogic.Models;
using NUnit.Framework;
using System.Linq;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.PlayedGameRepositoryTests
{
    [TestFixture]
    public class GetPlayedGameDetailsIntegrationTests : IntegrationTestBase
    {
        private PlayedGame GetTestSubjectPlayedGame(ApplicationDataContext dataContextToTestWith)
        {
            return new EntityFrameworkPlayedGameRepository(dataContextToTestWith)
                .GetPlayedGameDetails(testPlayedGames[0].Id, testUserWithDefaultGamingGroup);
        }

        [Test]
        public void ItRetrievesThePlayedGame()
        {
            using (ApplicationDataContext dbContext = new ApplicationDataContext())
            {
                PlayedGame playedGame = GetTestSubjectPlayedGame(dbContext);
                Assert.NotNull(playedGame);
            }
        }

        [Test]
        public void ItEagerlyFetchesTheGameResults()
        {
            using(NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                using (ApplicationDataContext dataContext = new ApplicationDataContext(dbContext, securedEntityValidatorFactory))
                {
                    PlayedGame playedGame = GetTestSubjectPlayedGame(dataContext);
                    Assert.GreaterOrEqual(testPlayedGames[0].PlayerGameResults.Count, playedGame.PlayerGameResults.Count());
                }
            }
        }

        [Test]
        public void ItEagerlyFetchesTheGameDefinition()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                using (ApplicationDataContext dataContext = new ApplicationDataContext(dbContext, securedEntityValidatorFactory))
                {
                    PlayedGame playedGame = GetTestSubjectPlayedGame(dataContext);
                    Assert.NotNull(playedGame.GameDefinition);
                }
            }
        }

        [Test]
        public void ItReturnsNullIfNoPlayedGameFound()
        {
            using (ApplicationDataContext dataContext = new ApplicationDataContext())
            {
                EntityFrameworkPlayedGameRepository playedGameRepository = new EntityFrameworkPlayedGameRepository(dataContext);

                PlayedGame notFoundPlayedGame = playedGameRepository.GetPlayedGameDetails(-1, testUserWithDefaultGamingGroup);
                Assert.Null(notFoundPlayedGame);
            }
        }

        [Test]
        public void ItThrowsANotAuthorizedExceptionIfTheGameIsntInThePlayersGamingGroup()
        {
            using (ApplicationDataContext dataContext = new ApplicationDataContext())
            {
                PlayedGame gameWithMismatchedGamingGroupId = testPlayedGames.First(
                    playedGame => playedGame.GamingGroupId != testUserWithDefaultGamingGroup.CurrentGamingGroupId);

                EntityFrameworkPlayedGameRepository playedGameRepository = new EntityFrameworkPlayedGameRepository(dataContext);

                PlayedGame notFoundPlayedGame = playedGameRepository.GetPlayedGameDetails(gameWithMismatchedGamingGroupId.Id, testUserWithDefaultGamingGroup);
                Assert.Null(notFoundPlayedGame);
            }
        }
    }
}
