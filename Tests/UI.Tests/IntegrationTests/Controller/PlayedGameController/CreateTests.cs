using BusinessLogic.DataAccess;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Tests.IntegrationTests.Controller.PlayedGameController
{
    [TestFixture]
    public class CreateTests
    {
        private NerdScorekeeperDbContext dbContext;

        [TestFixtureSetUp]
        public void SetUp()
        {
            dbContext = new NerdScorekeeperDbContext();
        }

        [Test]
        public void ItRequiresAGame()
        {

        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }
  
    }
}
