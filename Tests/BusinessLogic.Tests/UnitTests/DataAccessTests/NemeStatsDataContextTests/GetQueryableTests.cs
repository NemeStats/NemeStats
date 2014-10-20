using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using System.Data.Entity;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.NemeStatsDataContextTests
{
    [TestFixture]
    public class GetQueryableTests : NemeStatsDataContextTestBase
    {
        [Test]
        [Ignore("Having trouble testing this. Going to try an integration test instead.")]
        public void ItAutomaticallyFiltersOnGamingGroupIfTheEntityIsSecured()
        {
            DbSet<GameDefinition> dbSetMock = MockRepository.GenerateMock<DbSet<GameDefinition>>();
            //IQueryable<GameDefinition> gameDefinitionQueryable;
            nemeStatsDbContext.Expect(mock => mock.Set<GameDefinition>())
                .Repeat.Once()
                .Return(dbSetMock);

            //IQueryable<GameDefinition> gameDefinitionQueryable = dataContext.GetQueryable<GameDefinition>(currentUser);

            //TODO any way to check the predicate? or do I really just need an integration test here?
        }
    }
}
