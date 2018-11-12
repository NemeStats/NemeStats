using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.PlayedGames;

namespace BusinessLogic.DataAccess.Repositories
{
    public class PlayedGameRepository : IPlayedGameRepository
    {
        private readonly IDataContext _dataContext;

        public PlayedGameRepository(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        private const string RAW_SQL = @"WITH GamingGroupsWithGamesInLastTwoDays AS (
            SELECT DISTINCT PlayedGame.GamingGroupId, MAX(PlayedGame.ID) AS PlayedGameId
            FROM PlayedGame
            {0}
            WHERE PlayedGame.DatePlayed BETWEEN @MinDate AND @MaxDate
            {1}
            GROUP BY PlayedGame.GamingGroupId
            ),
            WinningPlayers AS (
            SELECT MAX(PlayerGameResult.PlayerId) AS WinningPlayerId, PlayerGameResult.PlayedGameId
            FROM PlayerGameResult INNER JOIN GamingGroupsWithGamesInLastTwoDays 
	            ON PlayerGameResult.PlayedGameId = GamingGroupsWithGamesInLastTwoDays.PlayedGameId
            WHERE PlayerGameREsult.GameRank = 1
            GROUP BY PlayerGameResult.PlayedGameId)

            SELECT TOP {2} PlayedGame.Id AS PlayedGameId, 
            PlayedGame.GamingGroupId,
            PlayedGame.DatePlayed,
            PlayedGame.WinnerType,
            GameDefinition.Id AS GameDefinitionId,
            WinningPlayers.WinningPlayerId,
            Player.Name AS WinningPlayerName,
            GamingGroup.Name AS GamingGroupName,
            BoardGameGeekGameDefinition.Thumbnail AS ThumbnailImageUrl,
            GameDefinition.Name AS GameDefinitionName
            FROM GamingGroupsWithGamesInLastTwoDays
            INNER JOIN PlayedGame ON PlayedGame.Id = GamingGroupsWithGamesInLastTwoDays.PlayedGameId
            INNER JOIN GamingGroup ON PlayedGame.GamingGroupId = GamingGroup.Id
            INNER JOIN GameDefinition ON PlayedGame.GameDefinitionId = GameDefinition.Id
            INNER JOIN BoardGameGeekGameDefinition ON GameDefinition.BoardGameGeekGameDefinitionId =  BoardGameGeekGameDefinition.Id
            INNER JOIN WinningPlayers ON WinningPlayers.PlayedGameId = GamingGroupsWithGamesInLastTwoDays.PlayedGameId
            LEFT JOIN Player on WinningPlayers.WinningPlayerId = Player.Id
            ORDER BY PlayedGameId DESC";

        public List<PublicGameSummary> GetRecentPublicGames(RecentlyPlayedGamesFilter filter)
        {
            string boardGameGeekGameDefinitionInnerJoin = string.Empty;
            string boardGameGeekGameDefinitionIdPredicate = string.Empty;
            object[] sqlParams;
            if (filter.BoardGameGeekGameDefinitionId.HasValue)
            {
                boardGameGeekGameDefinitionInnerJoin =
                    @"INNER JOIN GameDefinition ON PlayedGame.GameDefinitionId = GameDefinition.Id 
                    INNER JOIN BoardGameGeekGameDefinition ON BoardGameGeekGameDefinition.Id = GameDefinition.BoardGameGeekGameDefinitionId ";

                boardGameGeekGameDefinitionIdPredicate =
                    "AND BoardGameGeekGameDefinition.Id = @BoardGameGeekGameDefinitionId ";

                sqlParams = new object[4];
                sqlParams[3] = new SqlParameter("BoardGameGeekGameDefinitionId", filter.BoardGameGeekGameDefinitionId.Value);
            }
            else
            {
                sqlParams = new object[3];
            }
            sqlParams[0] = new SqlParameter("NumberOfResults", filter.NumberOfGamesToRetrieve);
            sqlParams[1] = new SqlParameter("MinDate", filter.MinDate ?? DateTime.UtcNow.AddYears(-10));
            sqlParams[2] = new SqlParameter("MaxDate", filter.MaxDate);

            var formattedSql = string.Format(RAW_SQL, 
                boardGameGeekGameDefinitionInnerJoin, 
                boardGameGeekGameDefinitionIdPredicate,
                filter.NumberOfGamesToRetrieve);

            var data = _dataContext.MakeRawSqlQuery<PublicGameSummary>(formattedSql, sqlParams);

            var results = data.ToList();
            return results;
        }
    }
}
