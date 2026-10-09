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
using System.Data.SqlClient;
using BusinessLogic.Caching;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.BoardGameGeekGameDefinitions;
using BusinessLogic.Logic.Utilities;
using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.BoardGameGeekGameDefinitions
{
    [TestFixture, Category("Integration")]
    public class UniversalStatsRetrieverIntegrationTests
    {
        private readonly string _databaseName = "NemeStatsUniversalStatsTests_" + Guid.NewGuid().ToString("N");
        private NemeStatsDataContext _dataContext;
        private UniversalStatsRetriever _retriever;

        [OneTimeSetUp]
        public void FixtureSetUp()
        {
            ExecuteSql("master", "CREATE DATABASE [" + _databaseName + "]");
            try
            {
                ExecuteSql(_databaseName, @"
                    CREATE TABLE BoardGameGeekGameDefinition (Id int NOT NULL PRIMARY KEY);
                    CREATE TABLE GameDefinition (
                        Id int NOT NULL PRIMARY KEY,
                        BoardGameGeekGameDefinitionId int NOT NULL,
                        GamingGroupId int NOT NULL,
                        Active bit NOT NULL);
                    CREATE TABLE PlayedGame (
                        Id int NOT NULL PRIMARY KEY,
                        GameDefinitionId int NOT NULL,
                        NumberOfPlayers int NOT NULL);
                    INSERT INTO BoardGameGeekGameDefinition VALUES (1),(2),(3),(4),(5),(6);
                    INSERT INTO GameDefinition VALUES
                        (11,1,1,1),(12,1,2,1),(13,1,3,1),
                        (31,3,1,1),(32,3,2,1),
                        (41,4,1,0),(42,4,2,1),(43,4,3,1),
                        (51,5,1,1),(52,5,2,1),
                        (61,6,1,1),(62,6,1,1);
                    INSERT INTO PlayedGame VALUES
                        (111,11,2),(112,11,2),(113,11,2),(121,12,3),
                        (411,41,4),(421,42,2),(511,51,3),(611,61,3),(621,62,5);");

                var dbContext = new NemeStatsDbContext();
                dbContext.Database.Connection.ConnectionString = ConnectionString(_databaseName);
                _dataContext = new NemeStatsDataContext(
                    dbContext, MockRepository.GenerateStub<ISecuredEntityValidatorFactory>());
                _retriever = new UniversalStatsRetriever(
                    MockRepository.GenerateStub<IDateUtilities>(),
                    MockRepository.GenerateStub<ICacheService>(),
                    _dataContext);
            }
            catch
            {
                _dataContext?.Dispose();
                ExecuteSql("master", "DROP DATABASE [" + _databaseName + "]");
                throw;
            }
        }

        [OneTimeTearDown]
        public void FixtureTearDown()
        {
            _dataContext?.Dispose();
            ExecuteSql("master", "DROP DATABASE [" + _databaseName + "]");
        }

        [TestCase(1, 2.5, 4, 2)]
        [TestCase(2, null, 0, 0)]
        [TestCase(3, 0.0, 0, 0)]
        [TestCase(4, 3.0, 2, 1)]
        [TestCase(5, 3.0, 1, 1)]
        [TestCase(6, 4.0, 2, 2)]
        public void It_Preserves_Statistics_For_Definition_And_Play_Boundaries(
            int boardGameGeekGameDefinitionId, double? averagePlayers, int totalPlays, int activeDefinitionsWithPlays)
        {
            var statistics = _retriever.GetFromSource(boardGameGeekGameDefinitionId);

            if (averagePlayers.HasValue)
            {
                Assert.That(statistics.AveragePlayersPerGame, Is.EqualTo(averagePlayers.Value).Within(0.000000001));
            }
            else
            {
                Assert.That(statistics.AveragePlayersPerGame, Is.Null);
            }
            Assert.That(statistics.TotalNumberOfGamesPlayed, Is.EqualTo(totalPlays));
            Assert.That(statistics.TotalGamingGroupsWithThisGame, Is.EqualTo(activeDefinitionsWithPlays));
        }

        [Test]
        public void It_Throws_If_The_BoardGameGeek_Game_Does_Not_Exist()
        {
            Assert.Throws<EntityDoesNotExistException<BoardGameGeekGameDefinition>>(
                () => _retriever.GetFromSource(-100));
        }

        private static string ConnectionString(string databaseName)
        {
            return new SqlConnectionStringBuilder
            {
                DataSource = @"(localdb)\MSSQLLocalDB",
                InitialCatalog = databaseName,
                IntegratedSecurity = true,
                Pooling = false
            }.ConnectionString;
        }

        private static void ExecuteSql(string databaseName, string sql)
        {
            using (var connection = new SqlConnection(ConnectionString(databaseName)))
            using (var command = new SqlCommand(sql, connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
