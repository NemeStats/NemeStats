using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
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

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.NemeStatsDataContextTests
{
    [TestFixture]
    public class DeleteByIdTests : NemeStatsDataContextTestBase
    {
        private SecuredEntityValidator<GamingGroup> securedEntityValidator;
        private DbSet<GamingGroup> gamingGroupDbSetMock;

        [SetUp]
        public void SetUp()
        {
            gamingGroupDbSetMock = MockRepository.GenerateMock<DbSet<GamingGroup>>();

            nemeStatsDbContext.Expect(mock => mock.Set<GamingGroup>())
                .Repeat.Once()
                .Return(gamingGroupDbSetMock);

            securedEntityValidator = MockRepository.GenerateMock<SecuredEntityValidator<GamingGroup>>();
            securedEntityValidatorFactory.Expect(mock => mock.MakeSecuredEntityValidator<GamingGroup>())
                .Repeat.Once()
                .Return(securedEntityValidator);
        }

        [Test]
        public void ItThrowsAKeyNotFoundExceptionIfTheIdDoesntHaveAMatchingEntity()
        {
            object emptyId = new object();
            gamingGroupDbSetMock.Expect(mock => mock.Find(emptyId))
                .Repeat.Once()
                .Return(null);

            Exception exception = Assert.Throws<KeyNotFoundException>(
                () => dataContext.DeleteById<GamingGroup>(emptyId, currentUser));

            string expectedMessage = string.Format(NemeStatsDataContext.EXCEPTION_MESSAGE_NO_ENTITY_EXISTS_FOR_THIS_ID, emptyId);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void ItValidatesThatTheUserHasAccessToDeleteTheEntity()
        {
            int id = 1;
            GamingGroup group = new GamingGroup() { Id = id };
            gamingGroupDbSetMock.Expect(mock => mock.Find(id))
                .Repeat.Once()
                .Return(group);

            dataContext.DeleteById<GamingGroup>(id, currentUser);

            securedEntityValidator.AssertWasCalled(mock => mock.ValidateAccess(
                Arg<GamingGroup>.Is.Same(group), 
                Arg<ApplicationUser>.Is.Same(currentUser), 
                Arg<Type>.Is.Equal(typeof(GamingGroup))));
        }
    }
}
