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

namespace BusinessLogic.Tests.UnitTests.DataAccessTests.EntityFrameworkGameDefinitionRepositoryTests
{
    [TestFixture]
    public class EntityFrameworkGameDefinitionRepositoryTestBase
    {
        protected EntityFrameworkGameDefinitionRepository gameDefinitionRepositoryPartialMock;
        protected NemeStatsDbContext dbContextMock;
        protected DbSet<GameDefinition> gameDefinitionsDbSetMock;
        protected UserContext userContext;
        protected GameDefinition gameDefinition;

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

            gameDefinitionRepositoryPartialMock = MockRepository.GeneratePartialMock<EntityFrameworkGameDefinitionRepository>(dbContextMock);
        }
    }
}
