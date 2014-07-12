using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.UnitTests.DataAccessTests
{
    [TestFixture]
    public class SaveTests
    {
        private EntityFrameworkGameDefinitionRepository gameDefinitionRepositoryPartialMock;
        private NemeStatsDbContext dbContextMock;
        DbSet<GameDefinition> gameDefinitionsDbSetMock;
        private UserContext userContext;
        private GameDefinition gameDefinition;

        [SetUp]
        public void SetUp()
        {
            gameDefinition = new GameDefinition()
            {
                Name = "game definition",
                Description = "game description"
            };
            userContext = new UserContext()
            {
                ApplicationUserId = "user id",
                GamingGroupId = 999
            };
            dbContextMock = MockRepository.GenerateMock<NemeStatsDbContext>();
            gameDefinitionsDbSetMock = MockRepository.GenerateMock<DbSet<GameDefinition>>();
            dbContextMock.Expect(mock => mock.GameDefinitions)
                .Repeat.Once()
                .Return(gameDefinitionsDbSetMock);
            gameDefinitionsDbSetMock.Expect(mock => mock.Add(gameDefinition));

            gameDefinitionRepositoryPartialMock = MockRepository.GeneratePartialMock<EntityFrameworkGameDefinitionRepository>(dbContextMock);
        }

        [Test]
        public void ItAddsTheGameDefinitionIfItIsntAlreadyInTheDatabase()
        {
            gameDefinitionRepositoryPartialMock.Save(gameDefinition, userContext);

            gameDefinitionsDbSetMock.AssertWasCalled(mock => mock.Add(gameDefinition));
            dbContextMock.AssertWasCalled(mock => mock.SaveChanges());
        }

        //TODO having trouble mocking dbEntityEntry. One more reason I need to implement a real repository pattern.
        /*
        [Test]
        public void ItMarksTheEntityAsModifiedIfItIsAlreadyInTheDatabase()
        {
            DbEntityEntry<GameDefinition> dbEntityEntry = MockRepository.GenerateMock<DbEntityEntry<GameDefinition>>();
            dbContextMock.Expect(mock => mock.Entry(gameDefinition))
                .Repeat.Once()
                .Return(dbEntityEntry);
            dbEntityEntry.Expect(mock => mock.State = EntityState.Modified);

            gameDefinitionRepositoryPartialMock.Save(gameDefinition, userContext);

            dbEntityEntry.VerifyAllExpectations();
            dbContextMock.AssertWasCalled(mock => mock.SaveChanges());
        }

        [Test]
        public void ItThrowsAnUnauthorizedAccessExceptionIfTheGameDefinitionIsExistingAndTheUserGamingGroupDoesNotMatch()
        {
            GameDefinition mismatchedGameDefinition = MockRepository.GeneratePartialMock<GameDefinition>();
            mismatchedGameDefinition.GamingGroupId = -1;
            mismatchedGameDefinition.Expect(mock => mock.AlreadyInDatabase())
                .Repeat.Once()
                .Return(true);
            gameDefinitionRepositoryPartialMock.Expect(mock => mock.ValidateUserHasAccessToGameDefinition(userContext, mismatchedGameDefinition));
            DbEntityEntry<GameDefinition> dbEntityEntry = MockRepository.GeneratePartialMock<DbEntityEntry<GameDefinition>>();
            dbContextMock.Expect(mock => mock.Entry(mismatchedGameDefinition))
                .Repeat.Once()
                .Return(dbEntityEntry);

            gameDefinitionRepositoryPartialMock.Save(mismatchedGameDefinition, userContext);

            gameDefinitionRepositoryPartialMock.AssertWasCalled(
                mock => mock.ValidateUserHasAccessToGameDefinition(userContext, mismatchedGameDefinition));
        }*/

        [Test]
        public void ItSetsTheGamingGroupIdToThatOfTheCurrentUser()
        {
            gameDefinitionRepositoryPartialMock.Save(gameDefinition, userContext);

            gameDefinitionsDbSetMock.AssertWasCalled(mock => mock.Add(
                Arg<GameDefinition>.Matches(
                    gameDef => gameDef.GamingGroupId == userContext.GamingGroupId)));
        }
    }
}
