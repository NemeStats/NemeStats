using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.PlayedGamesTests
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
                    PlayedGameRetrieverImpl retriever = new PlayedGameRetrieverImpl(dataContext);

                    List<PlayedGame> playedGames = retriever.GetRecentGames(1, testUserWithDefaultGamingGroup.CurrentGamingGroupId.Value);
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
                    PlayedGameRetrieverImpl retriever = new PlayedGameRetrieverImpl(dataContext);

                    List<PlayedGame> playedGames = retriever.GetRecentGames(1, testUserWithDefaultGamingGroup.CurrentGamingGroupId.Value);
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
                    PlayedGameRetrieverImpl retriever = new PlayedGameRetrieverImpl(dataContext);

                    List<PlayedGame> playedGames = retriever.GetRecentGames(1, testUserWithDefaultGamingGroup.CurrentGamingGroupId.Value);
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
                PlayedGameRetrieverImpl retriever = new PlayedGameRetrieverImpl(dataContext);
                int one = 1;
                List<PlayedGame> playedGames = retriever.GetRecentGames(one, testUserWithDefaultGamingGroup.CurrentGamingGroupId.Value);

                Assert.AreEqual(one, playedGames.Count());
            }
        }

        [Test]
        public void ItReturnsOnlyTwoGamesIfTwoGamesAreSpecified()
        {
            using (NemeStatsDataContext dataContext = new NemeStatsDataContext())
            {
                PlayedGameRetrieverImpl retriever = new PlayedGameRetrieverImpl(dataContext);
                int two = 2;
                List<PlayedGame> playedGames = retriever.GetRecentGames(two, testUserWithDefaultGamingGroup.CurrentGamingGroupId.Value);

                Assert.AreEqual(two, playedGames.Count());
            }
        }

        [Test]
        public void ItReturnsGamesInDescendingOrderByDatePlayed()
        {
            using (NemeStatsDataContext dataContext = new NemeStatsDataContext())
            {
                PlayedGameRetrieverImpl retriever = new PlayedGameRetrieverImpl(dataContext);
                int five = 5;
                List<PlayedGame> playedGames = retriever.GetRecentGames(five, testUserWithDefaultGamingGroup.CurrentGamingGroupId.Value);
                List<PlayedGame> allPlayedGames = dataContext.GetQueryable<PlayedGame>()
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
                PlayedGameRetrieverImpl retriever = new PlayedGameRetrieverImpl(dataContext);
                int five = 5;
                List<PlayedGame> playedGames = retriever.GetRecentGames(five, testUserWithDefaultGamingGroup.CurrentGamingGroupId.Value);

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
                PlayedGameRetrieverImpl retriever = new PlayedGameRetrieverImpl(dataContext);
                
                List<PlayedGame> playedGames = retriever.GetRecentGames(20, testUserWithOtherGamingGroup.CurrentGamingGroupId.Value);

                Assert.True(playedGames.All(game => game.GamingGroupId == testUserWithOtherGamingGroup.CurrentGamingGroupId));
            }
        }
    }
}
