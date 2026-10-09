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

using System.Data.SqlClient;
using System.Linq;
using BusinessLogic.Caching;
using BusinessLogic.DataAccess;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.Utilities;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;

namespace BusinessLogic.Logic.BoardGameGeekGameDefinitions
{
    public class UniversalStatsRetriever : Cacheable<int, UniversalGameStats>, IUniversalStatsRetriever
    {
        private const string SQL_GET_UNIVERSAL_GAME_STATS = @"
            WITH PerDefinition AS (
                SELECT gd.Id, gd.Active, COUNT(pg.Id) AS Plays,
                    AVG(CAST(pg.NumberOfPlayers AS float)) AS AveragePlayers
                FROM GameDefinition gd
                LEFT JOIN PlayedGame pg ON pg.GameDefinitionId = gd.Id
                WHERE gd.BoardGameGeekGameDefinitionId = @boardGameGeekGameDefinitionId
                GROUP BY gd.Id, gd.Active
            )
            SELECT
                CASE WHEN COUNT(d.Id) = 0 THEN CAST(NULL AS float)
                    ELSE COALESCE(AVG(d.AveragePlayers), 0) END AS AveragePlayersPerGame,
                COALESCE(SUM(d.Plays), 0) AS TotalNumberOfGamesPlayed,
                COALESCE(SUM(CASE WHEN d.Active = 1 AND d.Plays > 0 THEN 1 ELSE 0 END), 0)
                    AS TotalGamingGroupsWithThisGame
            FROM BoardGameGeekGameDefinition b
            LEFT JOIN PerDefinition d ON 1 = 1
            WHERE b.Id = @boardGameGeekGameDefinitionId
            GROUP BY b.Id;";

        private readonly IDataContext _dataContext;

        public UniversalStatsRetriever(IDateUtilities dateUtilities, ICacheService cacheService, IDataContext dataContext) : base(dateUtilities, cacheService)
        {
            _dataContext = dataContext;
        }

        public override UniversalGameStats GetFromSource(int boardGameGeekGameDefinitionId)
        {
            var statistics = _dataContext.MakeRawSqlQuery<UniversalGameStats>(
                SQL_GET_UNIVERSAL_GAME_STATS,
                new SqlParameter("boardGameGeekGameDefinitionId", boardGameGeekGameDefinitionId))
                .SingleOrDefault();

            if (statistics == null)
            {
                throw new EntityDoesNotExistException<BoardGameGeekGameDefinition>(boardGameGeekGameDefinitionId);
            }

            return statistics;
        }
    }
}
