using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Tests.IntegrationTests.DataAccessTests.RepositoriesTests.EntityFrameworkPlayerRepositoryTests
{
    [TestFixture]
    public class GetAllPlayersIntegrationTests : IntegrationTestBase
    {
        private IDataContext dataContext;
        private PlayerRetriever playerRetriever;

        [SetUp]
        public void TestSetUp()
        {
            dataContext = new NemeStatsDataContext();
            playerRetriever = new PlayerRetriever(dataContext);
        }

        [Test]
        public void ItOnlyReturnsActivePlayers()
        {
            List<Player> players = playerRetriever.GetAllPlayers(testUserWithDefaultGamingGroup.CurrentGamingGroupId.Value);

            Assert.True(players.All(x => x.Active));
        }

        [Test]
        public void ItOnlyReturnsPlayersForTheGivenGamingGroupId()
        {
            List<Player> players = playerRetriever.GetAllPlayers(testUserWithDefaultGamingGroup.CurrentGamingGroupId.Value);

            Assert.True(players.All(x => x.GamingGroupId == testGamingGroup.Id));
        }

        [TearDown]
        public void TearDown()
        {
            dataContext.Dispose();
        }
    }
}
