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
        private GameDefinitionSummary _gameDefinitionSummary;
        private int _numberOfGamesToRetrieve = 2;

        [OneTimeSetUp]
        public override void FixtureSetUp()
        {
            base.FixtureSetUp();

            var gameDefinitionRetriever = GetInstance<GameDefinitionRetriever>();
            GetInstance<IDataContext>().DetachEntities<GameDefinition>();

            _gameDefinitionSummary = gameDefinitionRetriever.GetGameDefinitionDetails(
                testGameDefinition.Id, 
                _numberOfGamesToRetrieve);
        }

        [Test]
        public void ItRetrievesTheSpecifiedGameDefinition()
        {
            Assert.AreEqual(testGameDefinition.Id, _gameDefinitionSummary.Id);
        }

        [Test]
        public void ItRetrievesTheLastXPlayedGames()
        {
            Assert.AreEqual(_numberOfGamesToRetrieve, _gameDefinitionSummary.PlayedGames.Count);
        }

        [Test]
        public void ItRetrievesGamesOrderedByDateDescending()
        {
            var lastDate = new DateTime(2600, 1, 1);
            foreach(var playedGame in _gameDefinitionSummary.PlayedGames)
            {
                Assert.LessOrEqual(playedGame.DatePlayed, lastDate);
                lastDate = playedGame.DatePlayed;
            }
        }

        [Test]
        public void ItRetrievesPlayerGameResultsForEachPlayedGame()
        {
            Assert.Greater(_gameDefinitionSummary.PlayedGames[0].PlayerGameResults.Count, 0);
        }

        [Test]
        public void ItRetrievesPlayerInfoForEachPlayerGameResult()
        {
            Assert.NotNull(_gameDefinitionSummary.PlayedGames[0].PlayerGameResults[0].Player);
        }

        [Test]
        public void ItRetrievesChampionInfoForTheGameDefinition()
        {
            Assert.That(_gameDefinitionSummary.ChampionId, Is.Not.Null);
            Assert.That(_gameDefinitionSummary.Champion, Is.Not.Null);
        }

        [Test]
        public void ItRetrievesThePlayerWinRecords()
        {
            //assumes there are 5 players - testplayer1 through testplayer 5
            Assert.That(_gameDefinitionSummary.PlayerWinRecords.Count, Is.EqualTo(5));
            Assert.That(_gameDefinitionSummary.PlayerWinRecords.Single(winRecord => winRecord.IsChampion), Is.Not.Null);
        }

        [Test]
        public void ItThrowsAnEntityDoesNotExistExceptionIfTheIdIsntValid()
        {
            int invalidId = -1;
            var expectedException = new EntityDoesNotExistException<GameDefinition>(invalidId);
            var gameDefinitionRetriever = GetInstance<GameDefinitionRetriever>();

            var actualException = Assert.Throws<EntityDoesNotExistException<GameDefinition>>(() => 
                gameDefinitionRetriever.GetGameDefinitionDetails(invalidId, 0));

            actualException.Message.ShouldBe(expectedException.Message);
        }
    }

    [TestFixture]
    public class CrossGroupPlayedGameNavigationIntegrationTests : IntegrationTestIoCBase
    {
        [Test]
        public void ItLoadsTheGamingGroupForPlayedGamesFromAnotherGroup()
        {
            var connectionString = System.Configuration.ConfigurationManager.AppSettings["Database.ConnectionString"];
            var connectionStringBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
            Assert.AreEqual("(localdb)\\NemeStatsIntegrationTests_20261011", connectionStringBuilder.DataSource);
            Assert.AreEqual("NerdScorekeeper", connectionStringBuilder.InitialCatalog);
            Assert.IsTrue(connectionStringBuilder.IntegratedSecurity);

            using (var context = GetInstance<BusinessLogic.DataAccess.NemeStatsDbContext>())
            {
                var groupIdSuffix = Guid.NewGuid().ToString("N");
                var owner = new BusinessLogic.Models.User.ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "local integration user " + groupIdSuffix,
                    Email = "local-integration-" + groupIdSuffix + "@example.invalid",
                    EmailConfirmed = false,
                    PhoneNumberConfirmed = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0
                };
                var definitionGroup = new GamingGroup { Name = "definition group " + groupIdSuffix, OwningUserId = owner.Id };
                var playedGameGroup = new GamingGroup { Name = "played-game group " + groupIdSuffix, OwningUserId = owner.Id };
                GameDefinition gameDefinition = null;
                PlayedGame playedGame = null;

                try
                {
                    context.Users.Add(owner);
                    context.SaveChanges();

                    context.GamingGroups.Add(definitionGroup);
                    context.GamingGroups.Add(playedGameGroup);
                    context.SaveChanges();

                    gameDefinition = new GameDefinition
                    {
                        Name = "test game definition " + groupIdSuffix,
                        GamingGroupId = definitionGroup.Id
                    };
                    context.GameDefinitions.Add(gameDefinition);
                    context.SaveChanges();

                    playedGame = new PlayedGame
                    {
                        GameDefinitionId = gameDefinition.Id,
                        GamingGroupId = playedGameGroup.Id,
                        DatePlayed = new DateTime(2078, 1, 1)
                    };
                    context.PlayedGames.Add(playedGame);
                    context.SaveChanges();

                    var retriever = GetInstance<GameDefinitionRetriever>();
                    var summary = retriever.GetGameDefinitionDetails(gameDefinition.Id, 1);
                    var retrievedPlayedGame = summary.PlayedGames.Single();

                    Assert.NotNull(retrievedPlayedGame.GamingGroup);
                    Assert.AreEqual(playedGameGroup.Id, retrievedPlayedGame.GamingGroup.Id);
                }
                finally
                {
                    using (var cleanupConnection = new System.Data.SqlClient.SqlConnection(connectionString))
                    {
                        cleanupConnection.Open();
                        using (var command = cleanupConnection.CreateCommand())
                        {
                            command.CommandText = @"
DELETE FROM dbo.PlayedGame WHERE Id = @playedGameId;
DELETE FROM dbo.GameDefinition WHERE Id = @gameDefinitionId;
DELETE FROM dbo.GamingGroup WHERE Id IN (@definitionGroupId, @playedGameGroupId);
DELETE FROM dbo.AspNetUsers WHERE Id = @ownerId;";
                            command.Parameters.AddWithValue("@playedGameId", playedGame?.Id ?? 0);
                            command.Parameters.AddWithValue("@gameDefinitionId", gameDefinition?.Id ?? 0);
                            command.Parameters.AddWithValue("@definitionGroupId", definitionGroup.Id);
                            command.Parameters.AddWithValue("@playedGameGroupId", playedGameGroup.Id);
                            command.Parameters.AddWithValue("@ownerId", owner.Id);
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
    }
}
