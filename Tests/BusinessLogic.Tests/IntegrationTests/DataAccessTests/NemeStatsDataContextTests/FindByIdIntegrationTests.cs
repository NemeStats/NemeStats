using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using NUnit.Framework;
using System.Linq;

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
            GameDefinition gameDefinition = dataContext.FindById<GameDefinition>(testGameDefinition.Id);

            Assert.NotNull(gameDefinition);
        }
    }
}
