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

using System;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using NUnit.Framework;
using Shouldly;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.GameDefinitionsTests.GameDefinitionRetrieverTests
{
    [TestFixture]
    public class GetGameDefinitionDetailsIntegrationTests : IntegrationTestBase
    {
        private NemeStatsDbContext dbContext;
        private IDataContext dataContext;
        private GameDefinitionSummary gameDefinitionSummary;
        private int numberOfGamesToRetrieve = 2;

        [OneTimeSetUp]
        public override void FixtureSetUp()
        {
            base.FixtureSetUp();

            using (dbContext = new NemeStatsDbContext())
            {
                using (dataContext = new NemeStatsDataContext(dbContext, securedEntityValidatorFactory))
                {
                    var playerRepository = new EntityFrameworkPlayerRepository(dataContext);

                    var gameDefinitionRetriever = new GameDefinitionRetriever(dataContext, playerRepository);
                    gameDefinitionSummary = gameDefinitionRetriever.GetGameDefinitionDetails(
                        testGameDefinition.Id, 
                        numberOfGamesToRetrieve);
                }
            }
        }

        [Test]
        public void ItRetrievesTheSpecifiedGameDefinition()
        {
            Assert.AreEqual(testGameDefinition.Id, gameDefinitionSummary.Id);
        }

        [Test]
        public void ItRetrievesTheLastXPlayedGames()
        {
            Assert.AreEqual(numberOfGamesToRetrieve, gameDefinitionSummary.PlayedGames.Count);
        }

        [Test]
        public void ItRetrievesGamesOrderedByDateDescending()
        {
            var lastDate = new DateTime(2100, 1, 1);
            foreach(var playedGame in gameDefinitionSummary.PlayedGames)
            {
                Assert.LessOrEqual(playedGame.DatePlayed, lastDate);
                lastDate = playedGame.DatePlayed;
            }
        }

        [Test]
        public void ItRetrievesPlayerGameResultsForEachPlayedGame()
        {
            Assert.Greater(gameDefinitionSummary.PlayedGames[0].PlayerGameResults.Count, 0);
        }

        [Test]
        public void ItRetrievesPlayerInfoForEachPlayerGameResult()
        {
            Assert.NotNull(gameDefinitionSummary.PlayedGames[0].PlayerGameResults[0].Player);
        }

        [Test]
        public void ItRetrievesChampionInfoForTheGameDefinition()
        {
            Assert.That(gameDefinitionSummary.Champion, Is.Not.Null);
        }

        [Test]
        public void ItRetrievesThePlayerWinRecords()
        {
            //assumes there are 5 players - testplayer1 through testplayer 5
            Assert.That(gameDefinitionSummary.PlayerWinRecords.Count, Is.EqualTo(5));
            Assert.That(gameDefinitionSummary.PlayerWinRecords.Single(winRecord => winRecord.IsChampion), Is.Not.Null);
        }

        [Test]
        public void ItThrowsAnEntityDoesNotExistExceptionIfTheIdIsntValid()
        {
            using (dbContext = new NemeStatsDbContext())
            {
                using (dataContext = new NemeStatsDataContext(dbContext, securedEntityValidatorFactory))
                {
                    var playerRepository = new EntityFrameworkPlayerRepository(dataContext);

                    int invalidId = -1;
                    var expectedException = new EntityDoesNotExistException(typeof(GameDefinition), invalidId);
                    var gameDefinitionRetriever = new GameDefinitionRetriever(dataContext, playerRepository);

                    var actualException = Assert.Throws<EntityDoesNotExistException>(() => 
                        gameDefinitionRetriever.GetGameDefinitionDetails(invalidId, 0));

                    actualException.Message.ShouldBe(expectedException.Message);
                }
            }
        }
    }
}
