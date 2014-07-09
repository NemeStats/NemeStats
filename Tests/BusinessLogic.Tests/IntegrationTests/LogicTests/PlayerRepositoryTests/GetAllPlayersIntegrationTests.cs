using BusinessLogic.DataAccess;
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
        private PlayerRepository playerRepository;

        [TestFixtureSetUp]
        public void SetUp()
        {
            dbContext = new NemeStatsDbContext();
        }

        [SetUp]
        public void TestSetUp()
        {
            playerRepository = new BusinessLogic.Models.PlayerRepository(dbContext);
        }

        [Test]
        public void ItOnlyReturnsActivePlayersWhenActivePlayersAreRequested()
        {
            bool active = true;

            List<Player> players = playerRepository.GetAllPlayers(active, testUserContextForUserWithDefaultGamingGroup);

            Assert.True(players.All(x => x.Active == active));
        }

        [Test]
        public void ItOnlyReturnsInactivePlayersWhenInActivePlayersAreRequested()
        {
            bool active = false;

            List<Player> players = playerRepository.GetAllPlayers(active, testUserContextForUserWithDefaultGamingGroup);

            Assert.True(players.All(x => x.Active == active));
        }

        [Test]
        public void ItOnlyReturnsPlayersForTheGivenGamingGroupId()
        {
            List<Player> players = playerRepository.GetAllPlayers(true, testUserContextForUserWithDefaultGamingGroup);

            Assert.True(players.All(x => x.GamingGroupId == gamingGroup.Id));
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }
    }
}
