using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
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
    public class ApplicationDataContextTestBase
    {
        protected NemeStatsDataContext dataContext;
        protected NemeStatsDbContext nemeStatsDbContext;
        protected SecuredEntityValidatorFactory securedEntityValidatorFactory;
        protected SecuredEntityValidator<EntityWithTechnicalKey> securedEntityValidator;
        protected EntityWithTechnicalKey entityWithGamingGroupAndTechnicalKey;
        protected EntityWithTechnicalKey entityWithGamingGroup;
        protected ApplicationUser currentUser;

        [SetUp]
        public void SetUp()
        {
            nemeStatsDbContext = MockRepository.GenerateMock<NemeStatsDbContext>();
            securedEntityValidatorFactory = MockRepository.GeneratePartialMock<SecuredEntityValidatorFactory>();
            dataContext = MockRepository.GeneratePartialMock<NemeStatsDataContext>(nemeStatsDbContext, securedEntityValidatorFactory);
            securedEntityValidator = MockRepository.GenerateMock<SecuredEntityValidator<EntityWithTechnicalKey>>();
            securedEntityValidatorFactory.Expect(mock => mock.MakeSecuredEntityValidator<EntityWithTechnicalKey>())
                .Repeat.Once()
                .Return(securedEntityValidator);
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
