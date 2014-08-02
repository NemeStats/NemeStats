using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic;
using BusinessLogic.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.PlayedGameRepositoryTests
{
    [TestFixture]
    public class GetRecentlyPlayedGameTests : IntegrationTestBase
    {
        [Test]
        public void ItEagerlyFetchesGameDefinitions()
        {
            using(NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Configuration.ProxyCreationEnabled = false;

                using (NemeStatsDataContext dataContext = new NemeStatsDataContext(dbContext, securedEntityValidatorFactory))
                {
                    PlayedGameRepository playedGameLogic = new EntityFrameworkPlayedGameRepository(dataContext);

                    List<PlayedGame> playedGames = playedGameLogic.GetRecentGames(1, testUserWithDefaultGamingGroup);
                    GameDefinition gameDefinition = playedGames[0].GameDefinition;

                    Assert.NotNull(gameDefinition);
                }
            }
        }

        [Test]
        public void ItEagerlyFetchesPlayerGameResults()
        {
            using(NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Configuration.ProxyCreationEnabled = false;
                using (NemeStatsDataContext dataContext = new NemeStatsDataContext(dbContext, securedEntityValidatorFactory))
                {
                    PlayedGameRepository playedGameLogic = new EntityFrameworkPlayedGameRepository(dataContext);

                    List<PlayedGame> playedGames = playedGameLogic.GetRecentGames(1, testUserWithDefaultGamingGroup);
                    ICollection<PlayerGameResult> playerGameResults = playedGames[0].PlayerGameResults;

                    Assert.NotNull(playerGameResults);
                }
            }
        }

        [Test]
        public void ItEagerlyFetchesPlayers()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Configuration.ProxyCreationEnabled = false;
                using (NemeStatsDataContext dataContext = new NemeStatsDataContext(dbContext, securedEntityValidatorFactory))
                {
                    PlayedGameRepository playedGameLogic = new EntityFrameworkPlayedGameRepository(dataContext);

                    List<PlayedGame> playedGames = playedGameLogic.GetRecentGames(1, testUserWithDefaultGamingGroup);
                    List<Player> players = playedGames[0].PlayerGameResults.Select(
                        playerGameResult => new Player()
                                                {
                                                    Id = playerGameResult.PlayerId,
                                                    Name = playerGameResult.Player.Name,
                                                    Active = playerGameResult.Player.Active
                                                }).ToList();

                    Assert.NotNull(players);
                }
            }
        }

        [Test]
        public void ItReturnsOnlyOneGameIfOneGameIsSpecified()
        {
            using (NemeStatsDataContext dataContext = new NemeStatsDataContext())
            {
                PlayedGameRepository playedGameLogic = new EntityFrameworkPlayedGameRepository(dataContext);
                int one = 1;
                List<PlayedGame> playedGames = playedGameLogic.GetRecentGames(one, testUserWithDefaultGamingGroup);

                Assert.AreEqual(one, playedGames.Count());
            }
        }

        [Test]
        public void ItReturnsOnlyTwoGamesIfTwoGamesAreSpecified()
        {
            using (NemeStatsDataContext dataContext = new NemeStatsDataContext())
            {
                PlayedGameRepository playedGameLogic = new EntityFrameworkPlayedGameRepository(dataContext);
                int two = 2;
                List<PlayedGame> playedGames = playedGameLogic.GetRecentGames(two, testUserWithDefaultGamingGroup);

                Assert.AreEqual(two, playedGames.Count());
            }
        }

        [Test]
        public void ItReturnsGamesInDescendingOrderByDatePlayed()
        {
            using (NemeStatsDataContext dataContext = new NemeStatsDataContext())
            {
                PlayedGameRepository playedGameLogic = new EntityFrameworkPlayedGameRepository(dataContext);
                int five = 5;
                List<PlayedGame> playedGames = playedGameLogic.GetRecentGames(five, testUserWithDefaultGamingGroup);
                List<PlayedGame> allPlayedGames = dataContext.GetQueryable<PlayedGame>(testUserWithDefaultGamingGroup)
                    .Where(game => game.GamingGroupId == testUserWithDefaultGamingGroup.CurrentGamingGroupId)
                    .ToList()
                    .OrderByDescending(playedGame => playedGame.DatePlayed)
                    .ToList();
                for(int i = 0; i<five; i++)
                {
                    Assert.AreEqual(allPlayedGames[i].Id, playedGames[i].Id);
                }
            }
        }

        [Test]
        public void ItReturnsOrderedPlayerRankDescendingWithinAGivenGame()
        {
            using (NemeStatsDataContext dataContext = new NemeStatsDataContext())
            {
                PlayedGameRepository playedGameLogic = new EntityFrameworkPlayedGameRepository(dataContext);
                int five = 5;
                List<PlayedGame> playedGames = playedGameLogic.GetRecentGames(five, testUserWithDefaultGamingGroup);

                int lastRank = -1;

                foreach(PlayedGame playedGame in playedGames)
                {
                    foreach(PlayerGameResult playerGameResult in playedGame.PlayerGameResults)
                    {
                        Assert.True(lastRank <= playerGameResult.GameRank);
                        lastRank = playerGameResult.GameRank;
                    }

                    lastRank = -1;
                }
            }
        }

        [Test]
        public void ItOnlyReturnsGamesForTheCurrentUsersGamingGroup()
        {
            using (NemeStatsDataContext dataContext = new NemeStatsDataContext())
            {
                PlayedGameRepository playedGameLogic = new EntityFrameworkPlayedGameRepository(dataContext);
                
                List<PlayedGame> playedGames = playedGameLogic.GetRecentGames(20, testUserWithOtherGamingGroup);

                Assert.True(playedGames.All(game => game.GamingGroupId == testUserWithOtherGamingGroup.CurrentGamingGroupId));
            }
        }
    }
}
