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
using System.Data.SqlClient;
using System.Linq;
using BusinessLogic.Models.Champions;
using BusinessLogic.Models.Games;

namespace BusinessLogic.DataAccess.Repositories
{
    public class ChampionRepository : IChampionRepository
    {
        private readonly IDataContext dataContext;

        public ChampionRepository(IDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        private const string CHAMPION_SQL =
            //--grab the top 2 so we can see if there is a tie... in which case nothing should change
            @"SELECT TOP 2 PlayerGameResult.PlayerId,
	             COUNT(PlayerGameResult.PlayerId) AS NumberOfGames,
	             SUM(CASE WHEN PlayerGameResult.GameRank = 1 THEN 1 ELSE 0 END) AS NumberOfWins,
				 CASE WHEN Champion.PlayerId = PlayerGameResult.PlayerId THEN 1 ELSE 0 END AS CurrentChampion
             FROM PlayerGameResult
             INNER JOIN PlayedGame
	             ON PlayerGameResult.PlayedGameId = PlayedGame.Id
             INNER JOIN Player
	             ON Player.Id = PlayerGameResult.PlayerId
	             AND Player.Active = 1
             INNER JOIN GameDefinition
	             ON GameDefinition.Id = PlayedGame.GameDefinitionId
		     LEFT JOIN Champion ON Champion.Id = GameDefinition.ChampionId
             WHERE PlayedGame.GameDefinitionId = @GameDefinitionId
             GROUP BY PlayerGameResult.PlayerId, 
			 CASE WHEN Champion.PlayerId = PlayerGameResult.PlayerId THEN 1 ELSE 0 END
             ORDER BY NumberOfWins DESC, NumberOfGames DESC, CurrentChampion DESC";

        public ChampionData GetChampionData(int gameDefinitionId)
        {
            var championStatisticsData = dataContext.MakeRawSqlQuery<ChampionStatistics>(CHAMPION_SQL,
                new SqlParameter("GameDefinitionId", gameDefinitionId));

            var championStatistics = championStatisticsData.ToList();

            if (ThereIsATieForChampion(championStatistics))
            {
                return new NullChampionData();
            }

            var championData = (from x in championStatistics
                                        select new ChampionData
                                        {
                                            PlayerId = x.PlayerId,
                                            NumberOfGames = x.NumberOfGames,
                                            NumberOfWins = x.NumberOfWins,
                                            WinPercentage = 100 * x.NumberOfWins / x.NumberOfGames,
                                            GameDefinitionId = gameDefinitionId
                                        }).FirstOrDefault();

            if (championData == null || championData.NumberOfWins == 0)
            {
                return new NullChampionData();
            }

            return championData;
        }

        private static bool ThereIsATieForChampion(List<ChampionStatistics> championStatistics)
        {
            if (championStatistics.Count != 2)
            {
                return false;
            }
            return !championStatistics[0].IsCurrentChampion && championStatistics[0].NumberOfGames == championStatistics[1].NumberOfGames
                   && championStatistics[0].NumberOfWins == championStatistics[1].NumberOfWins;
        }

        private const string USURPER_SQL =
           //--grab the top 2 so we can see if there is a tie... in which case nothing should change
           @"SELECT GameDefinitionId
                FROM Champion
                WHERE Champion.PlayerId = @PlayerId
                AND EXISTS 
                (
	                SELECT TOP 1 1 FROM Champion OtherChampionRecord 
	                WHERE OtherChampionRecord.GameDefinitionId = Champion.GameDefinitionId 
	                AND OtherChampionRecord.DateCreated < Champion.DateCreated 
	                AND OtherChampionRecord.PlayerId <> Champion.PlayerId
                )
                GROUP BY GameDefinitionId;";

        public List<int> GetUsurperAchievementData(int playerId)
        {
            var championStatisticsData = dataContext.MakeRawSqlQuery<int>(USURPER_SQL,
                new SqlParameter("PlayerId", playerId));

            return championStatisticsData.ToList();
        }
    }
}
