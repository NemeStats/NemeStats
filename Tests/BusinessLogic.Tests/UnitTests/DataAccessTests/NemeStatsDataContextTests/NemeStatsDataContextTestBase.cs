using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.NemeStatsDataContextTests
{
    public class NemeStatsDataContextTestBase
    {
        protected NemeStatsDataContext dataContext;
        protected NemeStatsDbContext nemeStatsDbContext;
        protected SecuredEntityValidatorFactory securedEntityValidatorFactory;
        protected EntityWithTechnicalKey entityWithGamingGroupAndTechnicalKey;
        protected EntityWithTechnicalKey entityWithGamingGroup;
        protected ApplicationUser currentUser;

        [SetUp]
        public void TestBaseSetUp()
        {
            nemeStatsDbContext = MockRepository.GenerateMock<NemeStatsDbContext>();
            securedEntityValidatorFactory = MockRepository.GeneratePartialMock<SecuredEntityValidatorFactory>();
            dataContext = MockRepository.GeneratePartialMock<NemeStatsDataContext>(nemeStatsDbContext, securedEntityValidatorFactory);
           
            entityWithGamingGroupAndTechnicalKey = MockRepository.GenerateStub<EntityWithTechnicalKey>();
            entityWithGamingGroupAndTechnicalKey.Expect(mock => mock.AlreadyInDatabase())
                .Repeat.Once()
                .Return(true);
            entityWithGamingGroup = MockRepository.GenerateStub<EntityWithTechnicalKey>();

            currentUser = new ApplicationUser()
            {
                Id = "application user id",
                CurrentGamingGroupId = 1
            };
        }
    }
}
