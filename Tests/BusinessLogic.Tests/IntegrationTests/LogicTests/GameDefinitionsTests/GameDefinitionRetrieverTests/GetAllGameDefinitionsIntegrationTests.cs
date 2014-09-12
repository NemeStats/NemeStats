using BusinessLogic.DataAccess;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.GameDefinitionsTests.GameDefinitionRetrieverTests
{
    [TestFixture]
    public class GetAllGameDefinitionsIntegrationTests : IntegrationTestBase
    {
        protected GameDefinitionRetriever retriever;
        protected IList<GameDefinition> actualGameDefinitions;

        [SetUp]
        public void SetUp()
        {
            using(NemeStatsDataContext dataContext = new NemeStatsDataContext())
            {
                retriever = new GameDefinitionRetriever(dataContext);
                actualGameDefinitions = retriever.GetAllGameDefinitions(testUserWithDefaultGamingGroup.CurrentGamingGroupId.Value);
            }
            
        }

        [Test]
        public void ItOnlyGetsGameDefinitionsForTheCurrentPlayersGamingGroup()
        {
            Assert.True(actualGameDefinitions.All(game => game.GamingGroupId == testUserWithDefaultGamingGroup.CurrentGamingGroupId));
        }

        [Test]
        public void ItSortsGameDefinitionsByNameAscending()
        {
            string previousName = null;

            foreach (GameDefinition gameDefinition in actualGameDefinitions)
            {
                if (previousName != null)
                {
                    Assert.LessOrEqual(previousName, gameDefinition.Name);
                }

                previousName = gameDefinition.Name;
            }
        }
    }
}
