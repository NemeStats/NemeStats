using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.IntegrationTests.DataAccessTests.GameDefinitionRepositoryTests
{
    public class GameDefinitionRepositoryTestBase : IntegrationTestBase
    {
        protected NemeStatsDbContext dbContext;
        protected GameDefinitionRepository gameDefinitionRepository;

        [SetUp]
        public void TestSetUp()
        {
            dbContext = new NemeStatsDbContext();
            gameDefinitionRepository = new EntityFrameworkGameDefinitionRepository(dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }
    }
}
