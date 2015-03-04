#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models;
using NUnit.Framework;
using System;
using System.Linq;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.GameDefinitionsTests.GameDefinitionRetrieverTestBase
{
    [TestFixture]
    public class GetGameDefinitionDetailsIntegrationTests : IntegrationTestBase
    {
        private NemeStatsDbContext dbContext;
        private IDataContext dataContext;
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
                    GameDefinitionRetriever gameDefinitionRetriever = new GameDefinitionRetriever(dataContext);
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

        [Test]
        public void ItRetrievesChampionInfoForTheGameDefinition()
        {
            Assert.That(gameDefinition.Champion, Is.Not.Null);
        }
    }
}
