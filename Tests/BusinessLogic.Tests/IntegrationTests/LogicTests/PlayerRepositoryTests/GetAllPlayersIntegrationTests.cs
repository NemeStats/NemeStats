using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic;
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
        private EntityFrameworkPlayerRepository playerRepository;

        [SetUp]
        public void TestSetUp()
        {
            dbContext = new NemeStatsDbContext();
            playerRepository = new EntityFrameworkPlayerRepository(dbContext);
        }

        [Test]
        public void ItOnlyReturnsActivePlayersWhenActivePlayersAreRequested()
        {
            bool active = true;

            List<Player> players = playerRepository.GetAllPlayers(active, testUserWithDefaultGamingGroup);

            Assert.True(players.All(x => x.Active == active));
        }

        [Test]
        public void ItOnlyReturnsInactivePlayersWhenInActivePlayersAreRequested()
        {
            bool active = false;

            List<Player> players = playerRepository.GetAllPlayers(active, testUserWithDefaultGamingGroup);

            Assert.True(players.All(x => x.Active == active));
        }

        [Test]
        public void ItOnlyReturnsPlayersForTheGivenGamingGroupId()
        {
            List<Player> players = playerRepository.GetAllPlayers(true, testUserWithDefaultGamingGroup);

            Assert.True(players.All(x => x.GamingGroupId == testGamingGroup.Id));
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }
    }
}
