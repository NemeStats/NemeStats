using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.ApplicationDataContextTests
{
    [TestFixture]
    public class GetQueryableTests : ApplicationDataContextTestBase
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
