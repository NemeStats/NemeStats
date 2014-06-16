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
    public class GetRecentlyPlayedGameTests
    {
        private NemeStatsDbContext dbContext;
        private PlayedGameLogic playedGameLogic;

        [SetUp]
        public void SetUp()
        {
            dbContext = new NemeStatsDbContext();
            playedGameLogic = new BusinessLogic.Models.PlayedGameRepository(dbContext);
        }

        [Test]
        public void ItEagerlyFetchesGameDefinitions()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            dbContext.Configuration.ProxyCreationEnabled = false;

            List<PlayedGame> playedGames = playedGameLogic.GetRecentGames(1);
            GameDefinition gameDefinition = playedGames[0].GameDefinition;

            Assert.NotNull(gameDefinition);
        }

        [Test]
        public void ItEagerlyFetchesPlayerGameResults()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            dbContext.Configuration.ProxyCreationEnabled = false;

            List<PlayedGame> playedGames = playedGameLogic.GetRecentGames(1);
            ICollection<PlayerGameResult> playerGameResults = playedGames[0].PlayerGameResults;

            Assert.NotNull(playerGameResults);
        }

        [Test]
        public void ItEagerlyFetchesPlayers()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            dbContext.Configuration.ProxyCreationEnabled = false;

            List<PlayedGame> playedGames = playedGameLogic.GetRecentGames(1);
            List<Player> players = playedGames[0].PlayerGameResults.Select(
                playerGameResult => new Player()
                                        {
                                            Id = playerGameResult.PlayerId,
                                            Name = playerGameResult.Player.Name,
                                            Active = playerGameResult.Player.Active
                                        }).ToList();
                                            
            Assert.NotNull(players);
        }

        [Test]
        public void ItReturnsOnlyOneGameIfOneGameIsSpecified()
        {
            int one = 1;
            List<PlayedGame> playedGames = playedGameLogic.GetRecentGames(one);

            Assert.AreEqual(one, playedGames.Count());
        }

        [Test]
        public void ItReturnsOnlyTwoGamesIfTwoGamesAreSpecified()
        {
            int two = 2;
            List<PlayedGame> playedGames = playedGameLogic.GetRecentGames(two);

            Assert.AreEqual(two, playedGames.Count());
        }

        [Test]
        public void ItReturnsGamesInDescendingOrderByDatePlayed()
        {
            int five = 5;
            List<PlayedGame> playedGames = playedGameLogic.GetRecentGames(five);

            DateTime lastGameDate = playedGames[0].DatePlayed;
            DateTime nextGameDate;

            for (int i = 1; i < five; i++ )
            {
                nextGameDate = playedGames[i].DatePlayed;
                Assert.True(lastGameDate >= nextGameDate);
                lastGameDate = nextGameDate;
            }
        }

        [Test]
        public void ItReturnsOrderedPlayerRankDescendingWithinAGivenGame()
        {
            int five = 5;
            List<PlayedGame> playedGames = playedGameLogic.GetRecentGames(five);

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

        [TestFixtureTearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }
    }
}
