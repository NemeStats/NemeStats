using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.IntegrationTests.DataAccessTests.RepositoriesTests.EntityFrameworkGameDefinitionRepositoryTests
{
    public class EntityFrameworkGameDefinitionRepositoryTestBase : IntegrationTestBase
    {
        protected NemeStatsDataContext dataContext;
        protected EntityFrameworkGameDefinitionRepository gameDefinitionRepository;

        [SetUp]
        public void TestSetUp()
        {
            dataContext = new NemeStatsDataContext();
            gameDefinitionRepository = new EntityFrameworkGameDefinitionRepository(dataContext);
        }

        [TearDown]
        public void TearDown()
        {
            dataContext.Dispose();
        }
    }
}
