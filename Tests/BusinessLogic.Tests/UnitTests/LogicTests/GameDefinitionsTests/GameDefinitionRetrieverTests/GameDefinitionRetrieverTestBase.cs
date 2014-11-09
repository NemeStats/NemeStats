using BusinessLogic.DataAccess;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Tests.UnitTests.LogicTests.GameDefinitionsTests.GameDefinitionRetrieverTests
{
    public class GameDefinitionRetrieverTestBase
    {
        protected IDataContext dataContext;
        protected ApplicationUser currentUser;
        protected IGameDefinitionRetriever retriever;
        protected IQueryable<GameDefinition> gameDefinitionQueryable;
        protected int gamingGroupId = 123;

        [SetUp]
        public void SetUp()
        {
            dataContext = MockRepository.GenerateMock<IDataContext>();
            currentUser = new ApplicationUser()
            {
                Id = "user id",
                CurrentGamingGroupId = gamingGroupId
            };
            List<GameDefinition> gameDefinitions = new List<GameDefinition>()
            {
                new GameDefinition() { Id = 1, Active = true, GamingGroupId = gamingGroupId, PlayedGames = new List<PlayedGame>()},  
                new GameDefinition() { Id = 2, Active = false, GamingGroupId = gamingGroupId, PlayedGames = new List<PlayedGame>() },
                new GameDefinition() { Id = 3, Active = true, GamingGroupId = -1, PlayedGames = new List<PlayedGame>() }

            };
            gameDefinitionQueryable = gameDefinitions.AsQueryable<GameDefinition>();

            retriever = new GameDefinitionRetriever(dataContext);
        }
    }
}
