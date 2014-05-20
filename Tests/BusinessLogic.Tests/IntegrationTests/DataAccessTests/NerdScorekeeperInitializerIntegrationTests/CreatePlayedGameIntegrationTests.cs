using BusinessLogic.DataAccess;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.IntegrationTests.DataAccess.NerdScorekeeperInitializerIntegrationTests
{
    //TODO Will i keep this test?
    /*
    [TestFixture]
    public class CreatePlayedGameIntegrationTests
    {
        private NerdScorekeeperDbContext dbContext = new NerdScorekeeperDbContext();
        private NerdScorekeeperInitializer initializer = new NerdScorekeeperInitializer();

        [SetUp]
        public void SetUp()
        {
            initializer.InitializeDatabase(dbContext);
        }

        [Test]
        public void ItCreatesAPlayedGame()
        {
            Assert.IsTrue(dbContext.PlayedGames.Count() > 0, "No played game was created");
        }
    }
     * */
}
