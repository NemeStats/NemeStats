using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.IntegrationTests.Logic.PlayerRepository
{
    [TestFixture]
    public class GetAllPlayersIntegrationTests
    {
        private NemeStatsDbContext dbContext;

        [TestFixtureSetUp]
        public void SetUp()
        {
            dbContext = new NemeStatsDbContext();
        }

        [Test]
        public void ItOnlyReturnsActivePlayersWhenActivePlayersAreRequested()
        {
            bool active = true;
            BusinessLogic.Logic.PlayerRepository playerRepository = new BusinessLogic.Logic.PlayerRepository(dbContext);

            List<Player> players = playerRepository.GetAllPlayers(active);

            Assert.True(players.All(x => x.Active == active));
        }

        [Test]
        public void ItOnlyReturnsInActivePlayersWhenInActivePlayersAreRequested()
        {
            bool active = false;
            BusinessLogic.Logic.PlayerRepository playerRepository = new BusinessLogic.Logic.PlayerRepository(dbContext);

            List<Player> players = playerRepository.GetAllPlayers(active);

            Assert.True(players.All(x => x.Active == active));
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }
    }
}
