using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.PlayerRepositoryTests
{
    [TestFixture]
    public class GetAllPlayersIntegrationTests : IntegrationTestBase
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
            BusinessLogic.Models.PlayerRepository playerRepository = new BusinessLogic.Models.PlayerRepository(dbContext);

            List<Player> players = playerRepository.GetAllPlayers(active);

            Assert.True(players.All(x => x.Active == active));
        }

        [Test]
        public void ItOnlyReturnsInactivePlayersWhenInActivePlayersAreRequested()
        {
            bool active = false;
            BusinessLogic.Models.PlayerRepository playerRepository = new BusinessLogic.Models.PlayerRepository(dbContext);

            List<Player> players = playerRepository.GetAllPlayers(active);

            Assert.True(players.All(x => x.Active == active));
        }

        //[Test]
        //public void ItOnlyReturnsPlayersForTheGivenGamingGroupId()
        //{
        //    bool active = false;
        //    BusinessLogic.Models.PlayerRepository playerRepository = new BusinessLogic.Models.PlayerRepository(dbContext);

        //    List<Player> players = playerRepository.GetAllPlayers(active, );

        //    Assert.True(players.All(x => x.Active == active));
        //}

        [TestFixtureTearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }
    }
}
