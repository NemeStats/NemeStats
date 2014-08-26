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
    public class FindByIdTests : NemeStatsDataContextTestBase
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

            GameDefinition actualGameDefinition = dataContext.FindById<GameDefinition>(entityId);

            Assert.AreSame(expectedGameDefinition, actualGameDefinition);
        }

        [Test]
        public void ItValidatesThatTheEntityExists()
        {
            int entityId = 1;
            GameDefinition gameDefinition = new GameDefinition() { Id = entityId };
            gameDefinitionDbSetMock.Expect(mock => mock.Find(entityId))
                .Return(gameDefinition);

            dataContext.FindById<GameDefinition>(entityId);

            dataContext.AssertWasCalled(mock => mock.ValidateEntityExists<GameDefinition>(entityId, gameDefinition));
        }
    }
}
