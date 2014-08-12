using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Models;
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
    public class FindByIdTests :NemeStatsDataContextTestBase
    {
        private SecuredEntityValidator<GameDefinition> securedEntityValidator;
        private DbSet<GameDefinition> gameDefinitionDbSetMock;

        [SetUp]
        public void SetUp()
        {
            gameDefinitionDbSetMock = MockRepository.GenerateMock<DbSet<GameDefinition>>();

            nemeStatsDbContext.Expect(mock => mock.Set<GameDefinition>())
                .Repeat.Once()
                .Return(gameDefinitionDbSetMock);

            securedEntityValidator = MockRepository.GenerateMock<SecuredEntityValidator<GameDefinition>>();
            securedEntityValidatorFactory.Expect(mock => mock.MakeSecuredEntityValidator<GameDefinition>())
                .Repeat.Once()
                .Return(securedEntityValidator);
        }

        [Test]
        public void ItReturnsTheEntityForTheGivenId()
        {
            int entityId = 1;
            GameDefinition expectedGameDefinition = new GameDefinition() { Id = entityId };
            gameDefinitionDbSetMock.Expect(mock => mock.Find(entityId))
                .Return(expectedGameDefinition);

            GameDefinition actualGameDefinition = dataContext.FindById<GameDefinition>(entityId, currentUser);

            Assert.AreSame(expectedGameDefinition, actualGameDefinition);
        }

        [Test]
        public void ItThrowsAKeyNotFoundExceptionIfTheEntityDoesntExist()
        {
            int invalidEntityId = -1;
            gameDefinitionDbSetMock.Expect(mock => mock.Find(invalidEntityId))
                .Return(null);

            Exception exception = Assert.Throws<KeyNotFoundException>(
                () => dataContext.FindById<GameDefinition>(invalidEntityId, currentUser));

            string expectedMessage = string.Format(
                NemeStatsDataContext.EXCEPTION_MESSAGE_NO_ENTITY_EXISTS_FOR_THIS_ID,
                invalidEntityId);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void ItThrowsAnUnauthorizedAccessExceptionIfTheUserDoesntHaveAccessToTheEntity()
        {
            int entityId = 1;
            GameDefinition gameDefinition = new GameDefinition() { Id = entityId };
            gameDefinitionDbSetMock.Expect(mock => mock.Find(entityId))
                .Return(gameDefinition);

            securedEntityValidator.Expect(mock => mock.ValidateAccess(gameDefinition, currentUser, typeof(GameDefinition), entityId))
                .Throw(new UnauthorizedAccessException());

            Exception exception = Assert.Throws<UnauthorizedAccessException>(
                () => dataContext.FindById<GameDefinition>(entityId, currentUser));
        }
    }
}
