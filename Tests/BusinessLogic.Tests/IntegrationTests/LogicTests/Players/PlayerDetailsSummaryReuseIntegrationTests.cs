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
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.DataAccess.Security;
using BusinessLogic.Logic.PlayerAchievements;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Paging;
using NUnit.Framework;
using PagedList;

namespace BusinessLogic.Tests.IntegrationTests.LogicTests.Players
{
    [TestFixture, Category("Integration")]
    public class PlayerDetailsSummaryReuseIntegrationTests
    {
        private readonly string _databaseName = "NemeStatsPlayerSummaryReuse_" + Guid.NewGuid().ToString("N");
        private NemeStatsDbContext _dbContext;
        private NemeStatsDataContext _dataContext;
        private PlayerRetriever _playerRetriever;
        private int _playerId;
        private int _playerWithoutGamesId;
        private int _gameDefinitionId;

        [OneTimeSetUp]
        public void FixtureSetUp()
        {
            try
            {
                ExecuteSql("master", "CREATE DATABASE [" + _databaseName + "]");
                _dbContext = new NemeStatsDbContext();
                _dbContext.Database.Connection.ConnectionString = ConnectionString(_databaseName);
                var schema = ((IObjectContextAdapter)_dbContext).ObjectContext.CreateDatabaseScript();
                schema = Regex.Replace(schema, "ON DELETE CASCADE", "ON DELETE NO ACTION", RegexOptions.IgnoreCase);
                ExecuteSql(_databaseName, schema);
                ExecuteSql(_databaseName,
                    "ALTER TABLE dbo.PlayerGameResult DROP COLUMN TotalPoints; " +
                    "ALTER TABLE dbo.PlayerGameResult ADD TotalPoints AS " +
                    "NemeStatsPointsAwarded + GameWeightBonusPoints + GameDurationBonusPoints;");
                SeedDatabase();

                _dataContext = new NemeStatsDataContext(_dbContext, new SecuredEntityValidatorFactory());
                _playerRetriever = new PlayerRetriever(
                    _dataContext,
                    new EntityFrameworkPlayerRepository(),
                    new EmptyRecentPlayerAchievementsUnlockedRetriever());
            }
            catch
            {
                _dataContext?.Dispose();
                _dbContext?.Dispose();
                DropDatabase();
                throw;
            }
        }

        [OneTimeTearDown]
        public void FixtureTearDown()
        {
            _dataContext?.Dispose();
            _dbContext?.Dispose();
            DropDatabase();
        }

        [Test]
        public void It_Executes_Player_Game_Summary_Query_Once_When_Getting_Player_Details()
        {
            var interceptor = new PlayerGameSummaryCommandInterceptor();
            DbInterception.Add(interceptor);
            try
            {
                var details = _playerRetriever.GetPlayerDetails(_playerId, 5);

                TestContext.WriteLine("PlayerGameSummarySqlCommandCount=" + interceptor.SummaryQueryCount);
                Assert.That(interceptor.SummaryQueryCount, Is.EqualTo(1));
                Assert.That(details.Name, Is.EqualTo("summary reuse player"));
                Assert.That(details.PlayerGameResults, Has.Count.EqualTo(1));
                Assert.That(details.PlayerStats.TotalGames, Is.EqualTo(1));
                Assert.That(details.PlayerStats.TotalGamesWon, Is.EqualTo(1));
                Assert.That(details.PlayerStats.TotalGamesLost, Is.EqualTo(0));
                Assert.That(details.PlayerStats.WinPercentage, Is.EqualTo(100));
                Assert.That(details.PlayerStats.AveragePlayersPerGame, Is.EqualTo(4));
                Assert.That(details.PlayerStats.NemePointsSummary.TotalPoints, Is.EqualTo(3));
                Assert.That(details.PlayerGameSummaries, Has.Count.EqualTo(1));
                Assert.That(details.PlayerGameSummaries[0].GameDefinitionId, Is.EqualTo(_gameDefinitionId));
                Assert.That(details.PlayerGameSummaries[0].NumberOfGamesWon, Is.EqualTo(1));
                Assert.That(details.PlayerGameSummaries[0].NumberOfGamesLost, Is.EqualTo(0));
                Assert.That(details.PlayerGameSummaries[0].WinPercentage, Is.EqualTo(100));
            }
            finally
            {
                DbInterception.Remove(interceptor);
            }
        }

        [Test]
        public void It_Preserves_Standalone_Player_Statistics()
        {
            var interceptor = new PlayerGameSummaryCommandInterceptor();
            DbInterception.Add(interceptor);
            try
            {
                var statistics = _playerRetriever.GetPlayerStatistics(_playerId);

                TestContext.WriteLine("StandalonePlayerGameSummarySqlCommandCount=" + interceptor.SummaryQueryCount);
                Assert.That(interceptor.SummaryQueryCount, Is.EqualTo(1));
                Assert.That(statistics.TotalGames, Is.EqualTo(1));
                Assert.That(statistics.TotalGamesWon, Is.EqualTo(1));
                Assert.That(statistics.TotalGamesLost, Is.EqualTo(0));
                Assert.That(statistics.WinPercentage, Is.EqualTo(100));
                Assert.That(statistics.AveragePlayersPerGame, Is.EqualTo(4));
                Assert.That(statistics.NemePointsSummary.TotalPoints, Is.EqualTo(3));
                Assert.That(statistics.GameDefinitionTotals.SummariesOfGameDefinitionTotals, Has.Count.EqualTo(1));
            }
            finally
            {
                DbInterception.Remove(interceptor);
            }
        }

        [Test]
        public void It_Returns_Zero_Standalone_Statistics_When_The_Player_Has_No_Games()
        {
            var interceptor = new PlayerGameSummaryCommandInterceptor();
            DbInterception.Add(interceptor);
            try
            {
                var statistics = _playerRetriever.GetPlayerStatistics(_playerWithoutGamesId);

                TestContext.WriteLine("EmptyPlayerGameSummarySqlCommandCount=" + interceptor.SummaryQueryCount);
                Assert.That(interceptor.SummaryQueryCount, Is.EqualTo(1));
                Assert.That(statistics.TotalGames, Is.EqualTo(0));
                Assert.That(statistics.TotalGamesWon, Is.EqualTo(0));
                Assert.That(statistics.TotalGamesLost, Is.EqualTo(0));
                Assert.That(statistics.WinPercentage, Is.EqualTo(0));
                Assert.That(statistics.AveragePlayersPerGame, Is.EqualTo(0));
                Assert.That(statistics.NemePointsSummary.TotalPoints, Is.EqualTo(0));
                Assert.That(statistics.GameDefinitionTotals.SummariesOfGameDefinitionTotals, Is.Empty);
            }
            finally
            {
                DbInterception.Remove(interceptor);
            }
        }

        private void SeedDatabase()
        {
            var gamingGroup = new GamingGroup { Name = "summary reuse group" };
            _dbContext.GamingGroups.Add(gamingGroup);
            _dbContext.SaveChanges();

            var player = new Player { Name = "summary reuse player", GamingGroupId = gamingGroup.Id };
            var playerWithoutGames = new Player { Name = "summary reuse player without games", GamingGroupId = gamingGroup.Id };
            var gameDefinition = new GameDefinition { Name = "summary reuse game", GamingGroupId = gamingGroup.Id };
            _dbContext.Players.Add(player);
            _dbContext.Players.Add(playerWithoutGames);
            _dbContext.GameDefinitions.Add(gameDefinition);
            _dbContext.SaveChanges();

            var playedGame = new PlayedGame
            {
                GamingGroupId = gamingGroup.Id,
                GameDefinitionId = gameDefinition.Id,
                NumberOfPlayers = 4,
                DatePlayed = new DateTime(2026, 10, 1),
                WinnerType = WinnerTypes.PlayerWin
            };
            _dbContext.PlayedGames.Add(playedGame);
            _dbContext.SaveChanges();

            _dbContext.PlayerGameResults.Add(new PlayerGameResult
            {
                PlayedGameId = playedGame.Id,
                PlayerId = player.Id,
                GameRank = 1,
                NemeStatsPointsAwarded = 3
            });
            _dbContext.SaveChanges();

            _playerId = player.Id;
            _playerWithoutGamesId = playerWithoutGames.Id;
            _gameDefinitionId = gameDefinition.Id;
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

        private void DropDatabase()
        {
            ExecuteSql("master",
                "IF DB_ID('" + _databaseName + "') IS NOT NULL BEGIN " +
                "ALTER DATABASE [" + _databaseName + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; " +
                "DROP DATABASE [" + _databaseName + "]; END");
        }

        private sealed class PlayerGameSummaryCommandInterceptor : DbCommandInterceptor
        {
            public int SummaryQueryCount { get; private set; }

            public override void ReaderExecuting(
                DbCommand command,
                DbCommandInterceptionContext<DbDataReader> interceptionContext)
            {
                if (command.CommandText.IndexOf("FROM [dbo].[GameDefinition]", StringComparison.OrdinalIgnoreCase) >= 0
                    && command.CommandText.IndexOf("AS NumberOfGamesWon", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    SummaryQueryCount++;
                }

                base.ReaderExecuting(command, interceptionContext);
            }
        }

        private sealed class EmptyRecentPlayerAchievementsUnlockedRetriever : IRecentPlayerAchievementsUnlockedRetriever
        {
            public IPagedList<PlayerAchievementWinner> GetResults(GetRecentPlayerAchievementsUnlockedQuery query)
            {
                return new List<PlayerAchievementWinner>().ToPagedList(query.Page, query.PageSize);
            }
        }
    }
}
