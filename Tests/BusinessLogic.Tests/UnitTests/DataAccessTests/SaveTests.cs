using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
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
        public void ItSavesTheGameDefinition()
        {
            gameDefinitionRepositoryPartialMock.Save(gameDefinition, userContext);

            gameDefinitionsDbSetMock.AssertWasCalled(mock => mock.Add(gameDefinition));
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

            gameDefinitionRepositoryPartialMock.Save(mismatchedGameDefinition, userContext);

            gameDefinitionRepositoryPartialMock.AssertWasCalled(
                mock => mock.ValidateUserHasAccessToGameDefinition(userContext, mismatchedGameDefinition));
        }

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
