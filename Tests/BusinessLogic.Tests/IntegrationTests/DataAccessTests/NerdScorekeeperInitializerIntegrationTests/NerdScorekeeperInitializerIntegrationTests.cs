using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.IntegrationTests.DataAccess.NerdScorekeeperInitializerIntegrationTests
{
    [TestFixture]
    public class NerdScorekeeperInitializerIntegrationTests
    {
        private NerdScorekeeperDbContext dbContext;

        [TestFixtureSetUp]
        public void SetUp()
        {
            dbContext = new NerdScorekeeperDbContext();
        }

        [Test]
        public void ItCreatesThreeGameDefinitions()
        {
            Assert.IsTrue(dbContext.GameDefinitions.Count() >= 3, "No GameDefinitions were created.");
        }

        [Test]
        public void ItCreatesThreePlayedGames()
        {
            List<PlayedGame> playedGames = dbContext.PlayedGames.ToList<PlayedGame>();
            Assert.IsTrue(dbContext.PlayedGames.Count() >= 3, "No played game was created.");
        }

        [Test]
        public void ItCreatesFivePlayers()
        {
            Assert.IsTrue(dbContext.Players.Count() >= 5, "No players were created.");
        }

        [Test]

        [TestFixtureTearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }
    }
}
