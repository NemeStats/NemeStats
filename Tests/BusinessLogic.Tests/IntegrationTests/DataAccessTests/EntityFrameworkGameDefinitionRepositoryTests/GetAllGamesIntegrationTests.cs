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

namespace BusinessLogic.Tests.IntegrationTests.DataAccessTests.EntityFrameworkGameDefinitionRepositoryTests
{
    [TestFixture]
    public class GetAllGamesIntegrationTests : EntityFrameworkGameDefinitionRepositoryTestBase
    {
        [Test]
        public void ItOnlyGetsGameDefinitionsForTheCurrentPlayersGamingGroup()
        {
            List<GameDefinition> gameDefinitions = gameDefinitionRepository.GetAllGameDefinitions(
                testUserContextForUserWithDefaultGamingGroup);

            Assert.True(gameDefinitions.All(game => game.GamingGroupId == testUserContextForUserWithDefaultGamingGroup.GamingGroupId));
        }
    }
}
