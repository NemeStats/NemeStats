using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.IntegrationTests.DataAccessTests.EntityFrameworkGameDefinitionRepositoryTests
{
    public class EntityFrameworkGamingGroupRepositoryTestBase : IntegrationTestBase
    {
        protected NemeStatsDbContext dbContext;
        protected EntityFrameworkGamingGroupRepository repository;

        [SetUp]
        public void TestSetUp()
        {
            dbContext = new NemeStatsDbContext();
            repository = new EntityFrameworkGamingGroupRepository(dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }
    }
}
