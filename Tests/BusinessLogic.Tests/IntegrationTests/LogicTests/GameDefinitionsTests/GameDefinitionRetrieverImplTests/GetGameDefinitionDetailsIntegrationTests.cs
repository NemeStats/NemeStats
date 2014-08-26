using BusinessLogic.DataAccess;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.GameDefinitionsTests.GameDefinitionRetrieverImplTestBase
{
    [TestFixture]
    public class GetGameDefinitionDetailsIntegrationTests : IntegrationTestBase
    {
        private NemeStatsDbContext dbContext;
        private DataContext dataContext;
        private GameDefinition gameDefinition;
        private int numberOfGamesToRetrieve = 2;

        [TestFixtureSetUp]
        public override void FixtureSetUp()
        {
            base.FixtureSetUp();

            using (dbContext = new NemeStatsDbContext())
            {
                using (dataContext = new NemeStatsDataContext(dbContext, securedEntityValidatorFactory))
                {
                    GameDefinitionRetrieverImpl gameDefinitionRetriever = new GameDefinitionRetrieverImpl(dataContext);
                    gameDefinition = gameDefinitionRetriever.GetGameDefinitionDetails(
                        testGameDefinition.Id, 
                        numberOfGamesToRetrieve);
                }
            }
        }

        [Test]
        public void ItRetrievesTheSpecifiedGameDefinition()
        {
            Assert.AreEqual(testGameDefinition.Id, gameDefinition.Id);
        }

        [Test]
        public void ItRetrievesTheLastXPlayedGames()
        {
            Assert.AreEqual(numberOfGamesToRetrieve, gameDefinition.PlayedGames.Count);
        }

        [Test]
        public void ItRetrievesGamesOrderedByDateDescending()
        {
            DateTime lastDate = new DateTime(2100, 1, 1);
            foreach(PlayedGame playedGame in gameDefinition.PlayedGames)
            {
                Assert.LessOrEqual(playedGame.DatePlayed, lastDate);
                lastDate = playedGame.DatePlayed;
            }
        }

        [Test]
        public void ItRetrievesPlayerGameResultsForEachPlayedGame()
        {
            Assert.Greater(gameDefinition.PlayedGames[0].PlayerGameResults.Count, 0);
        }

        [Test]
        public void ItRetrievesPlayerInfoForEachPlayerGameResult()
        {
            Assert.NotNull(gameDefinition.PlayedGames[0].PlayerGameResults[0].Player);
        }
    }
}
