using BusinessLogic.DataAccess;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.IntegrationTests.Logic
{
    [TestFixture]
    public class PlayerLogicIntegrationTests
    {
        private NerdScorekeeperDbContext dbContext;

        [TestFixtureSetUp]
        public void SetUp()
        {
            dbContext = new NerdScorekeeperDbContext();
        }

        [Test]
        public void Test1()
        {

        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

    }
}
