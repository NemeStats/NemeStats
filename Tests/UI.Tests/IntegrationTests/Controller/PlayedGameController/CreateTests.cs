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
        private NemeStatsDbContext dbContext;

        [TestFixtureSetUp]
        public void SetUp()
        {
            dbContext = new NemeStatsDbContext();
        }

        [Test]
        public void ItRequiresAGame()
        {
            //TODO THE SHAME!!!
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }
  
    }
}
