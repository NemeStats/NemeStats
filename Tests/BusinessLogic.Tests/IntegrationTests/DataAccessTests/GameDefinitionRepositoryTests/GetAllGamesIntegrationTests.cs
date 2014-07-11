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
    public class GetAllGamesIntegrationTests : IntegrationTestBase
    {
        private NemeStatsDbContext dbContext;
        private GameDefinitionRepository gameDefinitionRepository;

        [SetUp]
        public void TestSetUp()
        {
            dbContext = new NemeStatsDbContext();
            gameDefinitionRepository = new EntityFrameworkGameDefinitionRepository(dbContext);
        }

        //[Test]
        //public void ItOnlyGetsActiveGameDefinitions()
        //{
        //    List<GameDefinition> gameDefinitions = gameDefinitionRepository.GetAllGameDefinitions(dbContext, testUserContextForUserWithDefaultGamingGroup);

        //    Assert.True(gameDefinitions.All(game => game.Active))
        //}

        [Test]
        public void ItOnlyGetsGameDefinitionsForTheCurrentPlayersGamingGroup()
        {
            List<GameDefinition> gameDefinitions = gameDefinitionRepository.GetAllGameDefinitions(
                dbContext, 
                testUserContextForUserWithDefaultGamingGroup);

            Assert.True(gameDefinitions.All(game => game.GamingGroupId == testUserContextForUserWithDefaultGamingGroup.GamingGroupId));
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }
    }
}
