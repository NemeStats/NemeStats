using BusinessLogic.DataAccess;
using BusinessLogic.Logic;
using BusinessLogic.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.IntegrationTests.Logic.PlayedGameRepository
{
    [TestFixture]
    public class GetPlayedGameTests
    {
        private NemeStatsDbContext dbContext;
        private PlayedGameLogic playedGameLogic;

        [TestFixtureSetUp]
        public void SetUp()
        {
            dbContext = new NemeStatsDbContext();
            playedGameLogic = new BusinessLogic.Logic.PlayedGameRepository(dbContext);
        }

        [Test]
        public void ItGetsPlayedGamesWithGameDefinitionEagerlyFetched()
        {
            dbContext.Configuration.LazyLoadingEnabled = false;
            dbContext.Configuration.ProxyCreationEnabled = false;

            List<PlayedGame> playedGames = playedGameLogic.GetRecentGames(1);
            GameDefinition gameDefinition = playedGames[0].GameDefinition;

            Assert.NotNull(gameDefinition);
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

        [TestFixtureTearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }
    }
}
