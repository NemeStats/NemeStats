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

using BusinessLogic.Models.Nemeses;
using BusinessLogic.Models.Players;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;

namespace BusinessLogic.DataAccess.Repositories
{
    public class EntityFrameworkPlayerRepository : IPlayerRepository
    {
        public const int MINIMUM_NUMBER_OF_GAMES_TO_BE_A_NEMESIS = 3;

        private readonly IDataContext dataContext;

        public EntityFrameworkPlayerRepository(IDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        private const string SQL_GET_WIN_LOSS_GAMES_COUNT =
            @"SELECT SUM(NumberOfGamesLost) AS NumberOfGamesLost, SUM(NumberOfGamesWon) AS NumberOfGamesWon, 
                PlayerId as VersusPlayerId, PlayerName AS VersusPlayerName
            FROM
            (
	            SELECT COUNT(*) AS NumberOfGamesLost, 0 AS NumberOfGamesWon, PlayerId, PlayerName
	              FROM
	              (
		              SELECT OtherResults.PlayerId
					  ,Player.Name AS PlayerName
		              ,OtherResults.PlayedGameId
		              ,OtherResults.GameRank
		              , PlayedGame.GameDefinitionId
		              FROM PlayerGameResult Inner Join PlayedGame ON PlayergameResult.PlayedGameId = PlayedGame.Id
		              INNER JOIN PlayerGameResult OtherResults ON PlayedGame.Id = OtherResults.PlayedGameId
                      INNER JOIN Player ON OtherResults.PlayerId = Player.Id
		              WHERE PlayerGameResult.PlayerId = @PlayerId
		              AND OtherResults.GameRank < 
			            (
				            SELECT GameRank FROM PlayerGameResult PGR 
				            WHERE PGR.PlayedGameId = OtherResults.PlayedGameId 
				            AND PGR.PlayerId = @PlayerId
			            )
                        AND EXISTS (SELECT 1 FROM Player WHERE Player.Id = OtherResults.PlayerId AND Player.Active = 1)
	               ) AS LostGames
	               GROUP BY PlayerId, PlayerName
	               UNION
	               SELECT 0 AS NumberOfgamesLost, COUNT(*) AS NumberOfGamesWon, PlayerId, PlayerName
	              FROM
	              (
		              SELECT OtherResults.PlayerId
					  ,Player.Name AS PlayerName
		              ,OtherResults.PlayedGameId
		              ,OtherResults.GameRank
		              , PlayedGame.GameDefinitionId
		              FROM PlayerGameResult Inner Join PlayedGame ON PlayergameResult.PlayedGameId = PlayedGame.Id
		              INNER JOIN PlayerGameResult OtherResults ON PlayedGame.Id = OtherResults.PlayedGameId
                      INNER JOIN Player ON OtherResults.PlayerId = Player.Id
		              WHERE PlayerGameResult.PlayerId = @PlayerId
		              AND OtherResults.GameRank > 
			            (
				            SELECT GameRank FROM PlayerGameResult PGR 
				            WHERE PGR.PlayedGameId = OtherResults.PlayedGameId 
				            AND PGR.PlayerId = @PlayerId
			            ) 
                        AND EXISTS (SELECT 1 FROM Player WHERE Player.Id = OtherResults.PlayerId AND Player.Active = 1)
	               ) AS WonGames
	               GROUP BY PlayerId, PlayerName
            ) AS X
            GROUP BY PlayerId, PlayerName";

        public NemesisData GetNemesisData(int playerId)
        {
            DbRawSqlQuery<WinLossStatistics> data = dataContext.MakeRawSqlQuery<WinLossStatistics>(SQL_GET_WIN_LOSS_GAMES_COUNT,
                new SqlParameter("PlayerId", playerId));

            List<WinLossStatistics> winLossStatistics = data.ToList();

            NemesisData nemesisData = (from x in winLossStatistics
                          where x.NumberOfGamesLost > x.NumberOfGamesWon
                          && x.NumberOfGamesLost >= MINIMUM_NUMBER_OF_GAMES_TO_BE_A_NEMESIS
                          select new NemesisData
                          {
                              NumberOfGamesLost = x.NumberOfGamesLost,
                              LossPercentage = 100 * x.NumberOfGamesLost / (x.NumberOfGamesWon + x.NumberOfGamesLost),
                              NemesisPlayerId = x.VersusPlayerId
                          }).OrderByDescending(nemesisCandidates => nemesisCandidates.LossPercentage).FirstOrDefault();

            if(nemesisData == null)
            {
                return new NullNemesisData();
            }

            return nemesisData;
        }

        private const string SQL_GET_PLAYER_GAME_SUMMARY_INFO =
          @"SELECT GD.[Id] AS GameDefinitionId
          ,GD.[Name] AS GameDefinitionName
	      ,SUM(CASE WHEN PGR.GameRank = 1 THEN 1 ELSE 0 END) AS NumberOfGamesWon
          ,SUM(CASE WHEN PGR.GameRank <> 1 THEN 1 ELSE 0 END) AS NumberOfGamesLost
          FROM [dbo].[GameDefinition] GD 
          INNER JOIN PlayedGame PG ON GD.ID = PG.GameDefinitionID
          INNER JOIN PlayerGameResult PGR ON PG.ID = PGR.PlayedGameId
          WHERE PGR.PlayerId = @PlayerId
          GROUP BY GD.[Id], GD.[Name]
          ORDER BY NumberOfGamesWon DESC, NumberofGamesLost DESC, GameDefinitionName";

        public IList<PlayerGameSummary> GetPlayerGameSummaries(int playerId)
        {
            DbRawSqlQuery<PlayerGameSummary> data = dataContext.MakeRawSqlQuery<PlayerGameSummary>(SQL_GET_PLAYER_GAME_SUMMARY_INFO,
                new SqlParameter("PlayerId", playerId));

            var results = data.ToList();

            results.ForEach(record => record.WinPercentage = CalculateWinPercentage(record.NumberOfGamesWon, record.NumberOfGamesLost));

            return results;
        }

        public IList<PlayerVersusPlayerStatistics> GetPlayerVersusPlayersStatistics(int playerId)
        {
            DbRawSqlQuery<WinLossStatistics> data = dataContext.MakeRawSqlQuery<WinLossStatistics>(SQL_GET_WIN_LOSS_GAMES_COUNT,
                new SqlParameter("PlayerId", playerId));

            List<WinLossStatistics> winLossStatistics = data.OrderByDescending(x => x.NumberOfGamesLost + x.NumberOfGamesWon).ToList();

            return winLossStatistics.Select(winLossStats => new PlayerVersusPlayerStatistics
            {
                NumberOfGamesLostVersusThisPlayer = winLossStats.NumberOfGamesLost,
                NumberOfGamesWonVersusThisPlayer = winLossStats.NumberOfGamesWon,
                OpposingPlayerId = winLossStats.VersusPlayerId,
                OpposingPlayerName = winLossStats.VersusPlayerName
            }).ToList();
        }


        private const string SQL_GET_PLAYER_INFO_FOR_GIVEN_GAME_DEFINITION =
            @"SELECT
              Player.Id AS PlayerId
              ,Player.Name AS PlayerName
              ,SUM(CASE WHEN PGR.GameRank = 1 THEN 1 ELSE 0 END) AS GamesWon
              ,SUM(CASE WHEN PGR.GameRank <> 1 THEN 1 ELSE 0 END) AS GamesLost
              ,CONVERT(BIT, CASE WHEN Player.Id = Champion.PlayerId THEN 1 ELSE 0 END) AS IsChampion
              FROM [dbo].[GameDefinition] GD 
              INNER JOIN PlayedGame PG ON GD.ID = PG.GameDefinitionID
              INNER JOIN PlayerGameResult PGR ON PG.ID = PGR.PlayedGameId
              INNER JOIN Player ON PGR.PlayerId = Player.Id
              LEFT JOIN Champion ON Champion.Id = GD.ChampionId
              WHERE GD.Id = @GameDefinitionId
              GROUP BY GD.[Id], GD.[Name], Player.Id, Player.Name, CONVERT(BIT, CASE WHEN Player.Id = Champion.PlayerId THEN 1 ELSE 0 END)
              ORDER BY GamesWon DESC, GamesLost DESC, PlayerName";

        public IList<PlayerWinRecord> GetPlayerWinRecords(int gameDefinitionId)
        {
            DbRawSqlQuery<PlayerWinRecord> data = dataContext.MakeRawSqlQuery<PlayerWinRecord>(SQL_GET_PLAYER_INFO_FOR_GIVEN_GAME_DEFINITION,
                new SqlParameter("GameDefinitionId", gameDefinitionId));

            var results = data.ToList();

            results.ForEach(record => record.WinPercentage = CalculateWinPercentage(record.GamesWon, record.GamesLost));

            return results;
        }

        private static int CalculateWinPercentage(int gamesWon, int gamesLost)
        {
            if (gamesLost + gamesWon == 0)
            {
                return 0;
            }
            return (int)((decimal)gamesWon / (gamesLost + gamesWon) * 100);
        }
    }
}
