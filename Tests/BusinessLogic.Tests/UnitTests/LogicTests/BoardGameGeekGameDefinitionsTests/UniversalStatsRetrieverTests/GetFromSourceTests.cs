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

using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.BoardGameGeekGameDefinitions;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Framework;

namespace BusinessLogic.Tests.UnitTests.LogicTests.BoardGameGeekGameDefinitionsTests.UniversalStatsRetrieverTests
{
    public class GetFromSourceTests
    {
        [Test]
        public void It_Returns_The_Aggregated_Statistics_For_The_Requested_Game()
        {
            var statistics = new UniversalGameStats
            {
                AveragePlayersPerGame = 2.5,
                TotalNumberOfGamesPlayed = 4,
                TotalGamingGroupsWithThisGame = 2
            };
            var context = new TestDataContext(new[] { statistics });

            var result = MakeRetriever(context).GetFromSource(123);

            Assert.That(result.AveragePlayersPerGame, Is.EqualTo(2.5));
            Assert.That(result.TotalNumberOfGamesPlayed, Is.EqualTo(4));
            Assert.That(result.TotalGamingGroupsWithThisGame, Is.EqualTo(2));
            Assert.That(context.Parameters.Length, Is.EqualTo(1));
            var parameter = (SqlParameter)context.Parameters[0];
            Assert.That(parameter.ParameterName, Is.EqualTo("boardGameGeekGameDefinitionId"));
            Assert.That(parameter.Value, Is.EqualTo(123));
        }

        [Test]
        public void It_Throws_An_EntityDoesNotExistException_If_The_BoardGameGeekId_Doesnt_Map_To_A_Record()
        {
            var context = new TestDataContext(new UniversalGameStats[0]);

            Assert.Throws<EntityDoesNotExistException<BoardGameGeekGameDefinition>>(
                () => MakeRetriever(context).GetFromSource(-100));
        }

        private static UniversalStatsRetriever MakeRetriever(IDataContext context)
        {
            return new UniversalStatsRetriever(null, null, context);
        }

        private class TestDataContext : IDataContext
        {
            private readonly DbRawSqlQuery<UniversalGameStats> _query;
            public object[] Parameters { get; private set; }

            public TestDataContext(IEnumerable<UniversalGameStats> statistics)
            {
                _query = new TestQuery(statistics);
            }

            public DbRawSqlQuery<T> MakeRawSqlQuery<T>(string sql, params object[] parameters)
            {
                Parameters = parameters;
                return (DbRawSqlQuery<T>)(object)_query;
            }

            public void CommitAllChanges() { throw new System.NotSupportedException(); }
            public void MakeScarySqlAlteration(string sql, params object[] parameters) { throw new System.NotSupportedException(); }
            public IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class, IEntityWithTechnicalKey { throw new System.NotSupportedException(); }
            public TEntity FindById<TEntity>(object id) where TEntity : class, IEntityWithTechnicalKey { throw new System.NotSupportedException(); }
            public TEntity Save<TEntity>(TEntity entity, ApplicationUser currentUser) where TEntity : class, IEntityWithTechnicalKey { throw new System.NotSupportedException(); }
            public void Delete<TEntity>(TEntity entity, ApplicationUser currentUser) where TEntity : class, IEntityWithTechnicalKey { throw new System.NotSupportedException(); }
            public void DeleteById<TEntity>(object id, ApplicationUser currentUser) where TEntity : class, IEntityWithTechnicalKey { throw new System.NotSupportedException(); }
            public DbContextTransaction CurrentTransaction() { throw new System.NotSupportedException(); }
            public DbContextTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted) { throw new System.NotSupportedException(); }
            public void DetachEntities<TEntity>() where TEntity : class, IEntityWithTechnicalKey { throw new System.NotSupportedException(); }
            public TEntity AdminSave<TEntity>(TEntity entity) where TEntity : class, IEntityWithTechnicalKey { throw new System.NotSupportedException(); }
            public void SetCommandTimeout(int timeoutInSeconds) { throw new System.NotSupportedException(); }
            public void Dispose() { }
        }

        private class TestQuery : DbSqlQuery<UniversalGameStats>
        {
            private readonly IEnumerable<UniversalGameStats> _statistics;

            public TestQuery(IEnumerable<UniversalGameStats> statistics)
            {
                _statistics = statistics;
            }

            public override IEnumerator<UniversalGameStats> GetEnumerator() => _statistics.GetEnumerator();
        }
    }
}
