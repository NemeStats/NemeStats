using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            @"SELECT TOP 1 PlayerGameResult.PlayerId,
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
            DbRawSqlQuery<ChampionStatistics> championStatisticsData = dataContext.MakeRawSqlQuery<ChampionStatistics>(CHAMPION_SQL,
                new SqlParameter("GameDefinitionId", gameDefinitionId));

            List<ChampionStatistics> championStatistics = championStatisticsData.ToList();

            ChampionData championData = (from x in championStatistics
                                        select new ChampionData
                                        {
                                            PlayerId = x.PlayerId,
                                            NumberOfGames = x.NumberOfGames,
                                            NumberOfWins = x.NumberOfWins,
                                            WinPercentage = 100 * x.NumberOfWins / x.NumberOfGames,
                                            GameDefinitionId = gameDefinitionId
                                        }).FirstOrDefault();

            if (championData == null)
            {
                return new NullChampionData();
            }

            return championData;
        }
    }
}
