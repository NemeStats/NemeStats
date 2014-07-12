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
                PlayedGameLogic playedGameLogic = new EntityFrameworkPlayedGameRepository(dbContext);
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Configuration.ProxyCreationEnabled = false;

                List<PlayedGame> playedGames = playedGameLogic.GetRecentGames(1, testUserContextForUserWithDefaultGamingGroup);
                GameDefinition gameDefinition = playedGames[0].GameDefinition;

                Assert.NotNull(gameDefinition);
            }
        }

        [Test]
        public void ItEagerlyFetchesPlayerGameResults()
        {
            using(NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                PlayedGameLogic playedGameLogic = new EntityFrameworkPlayedGameRepository(dbContext);
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Configuration.ProxyCreationEnabled = false;

                List<PlayedGame> playedGames = playedGameLogic.GetRecentGames(1, testUserContextForUserWithDefaultGamingGroup);
                ICollection<PlayerGameResult> playerGameResults = playedGames[0].PlayerGameResults;

                Assert.NotNull(playerGameResults);
            }
        }

        [Test]
        public void ItEagerlyFetchesPlayers()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                PlayedGameLogic playedGameLogic = new EntityFrameworkPlayedGameRepository(dbContext);
                dbContext.Configuration.LazyLoadingEnabled = false;
                dbContext.Configuration.ProxyCreationEnabled = false;

                List<PlayedGame> playedGames = playedGameLogic.GetRecentGames(1, testUserContextForUserWithDefaultGamingGroup);
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

        [Test]
        public void ItReturnsOnlyOneGameIfOneGameIsSpecified()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                PlayedGameLogic playedGameLogic = new EntityFrameworkPlayedGameRepository(dbContext);
                int one = 1;
                List<PlayedGame> playedGames = playedGameLogic.GetRecentGames(one, testUserContextForUserWithDefaultGamingGroup);

                Assert.AreEqual(one, playedGames.Count());
            }
        }

        [Test]
        public void ItReturnsOnlyTwoGamesIfTwoGamesAreSpecified()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                PlayedGameLogic playedGameLogic = new EntityFrameworkPlayedGameRepository(dbContext);
                int two = 2;
                List<PlayedGame> playedGames = playedGameLogic.GetRecentGames(two, testUserContextForUserWithDefaultGamingGroup);

                Assert.AreEqual(two, playedGames.Count());
            }
        }

        [Test]
        public void ItReturnsGamesInDescendingOrderByDatePlayed()
        {
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                PlayedGameLogic playedGameLogic = new EntityFrameworkPlayedGameRepository(dbContext);
                int five = 5;
                List<PlayedGame> playedGames = playedGameLogic.GetRecentGames(five, testUserContextForUserWithDefaultGamingGroup);
                List<PlayedGame> allPlayedGames = dbContext.PlayedGames
                    .Where(game => game.GamingGroupId == testUserContextForUserWithDefaultGamingGroup.GamingGroupId)
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
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                PlayedGameLogic playedGameLogic = new EntityFrameworkPlayedGameRepository(dbContext);
                int five = 5;
                List<PlayedGame> playedGames = playedGameLogic.GetRecentGames(five, testUserContextForUserWithDefaultGamingGroup);

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
            using (NemeStatsDbContext dbContext = new NemeStatsDbContext())
            {
                PlayedGameLogic playedGameLogic = new EntityFrameworkPlayedGameRepository(dbContext);
                
                List<PlayedGame> playedGames = playedGameLogic.GetRecentGames(20, testUserContextForUserWithOtherGamingGroup);

                Assert.True(playedGames.All(game => game.GamingGroupId == testUserContextForUserWithOtherGamingGroup.GamingGroupId));
            }
        }
    }
}
