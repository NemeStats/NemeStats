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
        protected DataContext dataContextMock;
        protected IQueryable<GameDefinition> queryableGameDefinitions;
        protected ApplicationUser currentUser;
        protected GameDefinition gameDefinition;

        [SetUp]
        public virtual void SetUp()
        {
            gameDefinition = new GameDefinition()
            {
                Name = "game definition",
                Description = "game description",
                GamingGroupId = 999
            };
            currentUser = new ApplicationUser()
            {
                Id = "user id",
                CurrentGamingGroupId = gameDefinition.GamingGroupId
            };
            dataContextMock = MockRepository.GenerateMock<DataContext>();
            queryableGameDefinitions = MockRepository.GenerateMock<IQueryable<GameDefinition>>();

            gameDefinitionRepositoryPartialMock = MockRepository.GeneratePartialMock<EntityFrameworkGameDefinitionRepository>(dataContextMock);
        }
    }
}
