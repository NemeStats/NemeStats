using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.DataAccess.Security;
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
        protected SecuredEntityValidatorFactory validatorFactory;

        [SetUp]
        public void TestSetUp()
        {
            dataContext = new NemeStatsDataContext();
            validatorFactory = new SecuredEntityValidatorFactory();
            gameDefinitionRepository = new EntityFrameworkGameDefinitionRepository(dataContext, validatorFactory);
        }

        [TearDown]
        public void TearDown()
        {
            dataContext.Dispose();
        }
    }
}
