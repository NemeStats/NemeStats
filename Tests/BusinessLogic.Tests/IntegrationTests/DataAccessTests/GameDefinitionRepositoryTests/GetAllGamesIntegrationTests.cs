using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models;
using BusinessLogic.Tests.IntegrationTests.LogicTests;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.IntegrationTests.DataAccessTests.GameDefinitionRepositoryTests
{
    [TestFixture]
    public class GetAllGamesIntegrationTests : GameDefinitionRepositoryTestBase
    {
        [Test]
        public void ItOnlyGetsGameDefinitionsForTheCurrentPlayersGamingGroup()
        {
            List<GameDefinition> gameDefinitions = gameDefinitionRepository.GetAllGameDefinitions(
                dbContext, 
                testUserContextForUserWithDefaultGamingGroup);

            Assert.True(gameDefinitions.All(game => game.GamingGroupId == testUserContextForUserWithDefaultGamingGroup.GamingGroupId));
        }
    }
}
