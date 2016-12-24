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

using BusinessLogic.Logic.Utilities;
using BusinessLogic.Models.Nemeses;
using BusinessLogic.Models.Players;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using BusinessLogic.Models.Points;

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
                PlayerId as VersusPlayerId, PlayerName AS VersusPlayerName, PlayerActive AS VersusPlayerActive
            FROM
            (
	            SELECT COUNT(*) AS NumberOfGamesLost, 0 AS NumberOfGamesWon, PlayerId, PlayerName, PlayerActive
	              FROM
	              (
		              SELECT OtherResults.PlayerId
					  ,Player.Name AS PlayerName
                      ,Player.Active AS PlayerActive
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
                        AND EXISTS (SELECT 1 FROM Player WHERE Player.Id = OtherResults.PlayerId)
	               ) AS LostGames
	               GROUP BY PlayerId, PlayerName, PlayerActive
	               UNION
	               SELECT 0 AS NumberOfgamesLost, COUNT(*) AS NumberOfGamesWon, PlayerId, PlayerName, PlayerActive
	              FROM
	              (
		              SELECT OtherResults.PlayerId
					  ,Player.Name AS PlayerName
                      ,Player.Active AS PlayerActive
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
                        AND EXISTS (SELECT 1 FROM Player WHERE Player.Id = OtherResults.PlayerId)
	               ) AS WonGames
	               GROUP BY PlayerId, PlayerName, PlayerActive
            ) AS X
            GROUP BY PlayerId, PlayerName, PlayerActive";

        public NemesisData GetNemesisData(int playerId)
        {
            var data = dataContext.MakeRawSqlQuery<WinLossStatistics>(SQL_GET_WIN_LOSS_GAMES_COUNT,
                new SqlParameter("PlayerId", playerId));

            var winLossStatistics = data.ToList();

            var nemesisData = (from x in winLossStatistics
                          where x.NumberOfGamesLost > x.NumberOfGamesWon
                          && x.NumberOfGamesLost >= MINIMUM_NUMBER_OF_GAMES_TO_BE_A_NEMESIS
                          && x.VersusPlayerActive
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
          ,BGGGD.[Thumbnail] AS ThumbnailImageUrl
	      ,SUM(CASE WHEN PGR.GameRank = 1 THEN 1 ELSE 0 END) AS NumberOfGamesWon
          ,SUM(CASE WHEN PGR.GameRank <> 1 THEN 1 ELSE 0 END) AS NumberOfGamesLost
          FROM [dbo].[GameDefinition] GD 
          INNER JOIN PlayedGame PG ON GD.ID = PG.GameDefinitionID
          INNER JOIN PlayerGameResult PGR ON PG.ID = PGR.PlayedGameId
          LEFT JOIN BoardGameGeekGameDefinition BGGGD ON BGGGD.Id = GD.BoardGameGeekGameDefinitionId
          WHERE PGR.PlayerId = @PlayerId 
          GROUP BY GD.[Id], GD.[Name], BGGGD.[Thumbnail]
          ORDER BY NumberOfGamesWon DESC, NumberofGamesLost DESC, GameDefinitionName";

        public IList<PlayerGameSummary> GetPlayerGameSummaries(int playerId)
        {
            var data = dataContext.MakeRawSqlQuery<PlayerGameSummary>(SQL_GET_PLAYER_GAME_SUMMARY_INFO,
                new SqlParameter("PlayerId", playerId));

            var results = data.ToList();

            results.ForEach(record => record.WinPercentage = WinPercentageCalculator.CalculateWinPercentage(record.NumberOfGamesWon, record.NumberOfGamesLost));

            return results;
        }

        public IList<PlayerVersusPlayerStatistics> GetPlayerVersusPlayersStatistics(int playerId)
        {
            var data = dataContext.MakeRawSqlQuery<WinLossStatistics>(SQL_GET_WIN_LOSS_GAMES_COUNT,
                new SqlParameter("PlayerId", playerId));

            var winLossStatistics = data
                .OrderByDescending(x => x.VersusPlayerActive)
                .ThenByDescending(x => x.NumberOfGamesLost + x.NumberOfGamesWon).ToList();

            return winLossStatistics.Select(winLossStats => new PlayerVersusPlayerStatistics
            {
                NumberOfGamesLostVersusThisPlayer = winLossStats.NumberOfGamesLost,
                NumberOfGamesWonVersusThisPlayer = winLossStats.NumberOfGamesWon,
                OpposingPlayerId = winLossStats.VersusPlayerId,
                OpposingPlayerName = winLossStats.VersusPlayerName,
                OpposingPlayerActive = winLossStats.VersusPlayerActive
            }).ToList();
        }

        private const string SQL_GET_PLAYER_INFO_FOR_GIVEN_GAME_DEFINITION =
            @"SELECT
              Player.Id AS PlayerId
              ,Player.Name AS PlayerName
              ,Player.Active AS PlayerActive
              ,SUM(CASE WHEN PGR.GameRank = 1 THEN 1 ELSE 0 END) AS GamesWon
              ,SUM(CASE WHEN PGR.GameRank <> 1 THEN 1 ELSE 0 END) AS GamesLost
              ,SUM(PGR.NemeStatsPointsAwarded) AS BaseNemePoints
              ,SUM(PGR.GameWeightBonusPoints) AS WeightBonusNemePoints
              ,SUM(PGR.GameDurationBonusPoints) AS GameDurationBonusNemePoints
              ,CONVERT(BIT, CASE WHEN Player.Id = Champion.PlayerId THEN 1 ELSE 0 END) AS IsChampion
              ,CONVERT(BIT, CASE WHEN EXISTS(
                    SELECT 1 FROM Champion 
                    WHERE Champion.PlayerId = Player.Id 
                    AND Champion.GameDefinitionId = GD.Id) 
                        THEN 1 ELSE 0 END) AS IsFormerChampion
              FROM [dbo].[GameDefinition] GD 
              INNER JOIN PlayedGame PG ON GD.ID = PG.GameDefinitionID
              INNER JOIN PlayerGameResult PGR ON PG.ID = PGR.PlayedGameId
              INNER JOIN Player ON PGR.PlayerId = Player.Id
              LEFT JOIN Champion ON Champion.Id = GD.ChampionId
              WHERE GD.Id = @GameDefinitionId
              GROUP BY GD.[Id], GD.[Name], Player.Id, Player.Name,Player.Active, CONVERT(BIT, CASE WHEN Player.Id = Champion.PlayerId THEN 1 ELSE 0 END)
              ORDER BY PlayerActive DESC, BaseNemePoints DESC, GamesWon DESC, PlayerName";

        internal class PlayerWinRecordWithFlatPoints : PlayerWinRecord
        {
            public int BaseNemePoints { get; set; }
            public int WeightBonusNemePoints { get; set; }
            public int GameDurationBonusNemePoints { get; set; }
        }

        public IList<PlayerWinRecord> GetPlayerWinRecords(int gameDefinitionId)
        {
            var data = dataContext.MakeRawSqlQuery<PlayerWinRecordWithFlatPoints>(SQL_GET_PLAYER_INFO_FOR_GIVEN_GAME_DEFINITION,
                new SqlParameter("GameDefinitionId", gameDefinitionId));

            var results = data.Select(x => new PlayerWinRecord
            {
                PlayerId = x.PlayerId,
                NemePointsSummary = new NemePointsSummary(x.BaseNemePoints, x.GameDurationBonusNemePoints, x.WeightBonusNemePoints),
                TotalGamesPlayed = x.TotalGamesPlayed,
                AveragePointsPerGame = x.AveragePointsPerGame,
                PlayerName = x.PlayerName,
                GamesWon = x.GamesWon,
                WinPercentage = x.WinPercentage,
                PlayerActive = x.PlayerActive,
                GamesLost = x.GamesLost,
                IsChampion = x.IsChampion,
                IsFormerChampion = x.IsFormerChampion
            }).ToList();

            results.ForEach(record =>
            {
                record.WinPercentage = WinPercentageCalculator.CalculateWinPercentage(record.GamesWon, record.GamesLost);
                record.TotalGamesPlayed = record.GamesLost + record.GamesWon;
                if (record.TotalGamesPlayed > 0)
                {
                    record.AveragePointsPerGame = record.TotalGamesPlayed > 0 ? (float)record.NemePointsSummary.TotalPoints / record.TotalGamesPlayed : 0;
                }
            });

            return results;
        }

        private const string SQL_GET_LONGEST_WINNING_STREAK_FOR_PLAYER =
            @"WITH cte AS 
	            (SELECT PlayerGameResult.PlayerId, PlayerGameResult.GameRank, PlayedGame.DatePlayed,
	            ROW_NUMBER() OVER (PARTITION BY PlayerGameResult.PlayerId ORDER BY PlayedGame.DatePlayed, PlayedGame.Id) 
                    - ROW_NUMBER() OVER (PARTITION BY PlayerId, GameRank ORDER BY PlayedGame.DatePlayed, PlayedGame.Id) AS [GroupID]
	            FROM PlayerGameResult 
	            JOIN PlayedGame on PlayedGame.Id = PlayerGameResult.PlayedGameId
	            WHERE PlayerGameResult.PlayerId = @PlayerId),
            -- Calculate Wins/Loses totals per each playerId
	            cte1 AS (SELECT PlayerId, 
	            SUM(CASE WHEN GameRank = 1 THEN 1 ELSE 0 END) AS [Wins],
	            GroupId	
	            FROM cte
	            GROUP BY PlayerId, GroupID),
            -- Rank from highest to lowest wins
	            cteSummary AS (SELECT *, RANK() OVER (ORDER BY Wins DESC) AS WinsRank
	            FROM cte1)
            -- Get final results
	            SELECT [Wins]
	             FROM cteSummary WHERE WinsRank = 1";

        public int GetLongestWinningStreak(int playerId)
        {
            var longestWinningStreak = dataContext.MakeRawSqlQuery<int?>(SQL_GET_LONGEST_WINNING_STREAK_FOR_PLAYER,
                                                    new SqlParameter("PlayerId", playerId)).FirstOrDefault();

            if (longestWinningStreak == null)
            {
                return 0;
            }

            return longestWinningStreak.Value;
        }
    }
}
