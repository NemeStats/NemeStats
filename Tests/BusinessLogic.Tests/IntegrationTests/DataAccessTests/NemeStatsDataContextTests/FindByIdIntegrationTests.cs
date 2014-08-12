using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.IntegrationTests.DataAccessTests.NemeStatsDataContextTests
{
    [TestFixture]
    public class FindByIdIntegrationTests : IntegrationTestBase
    {
        protected NemeStatsDataContext dataContext;

        [SetUp]
        public void SetUp()
        {
            dataContext = new NemeStatsDataContext();
        }

        [TearDown]
        public void TearDown()
        {
            dataContext.Dispose();
        }

        [Test]
        public void ItCanFindAnEntityUsingAnIntId()
        {
            GameDefinition gameDefinition = dataContext.FindById<GameDefinition>(testGameDefinition.Id, testUserWithDefaultGamingGroup);

            Assert.NotNull(gameDefinition);
        }
    }
}
