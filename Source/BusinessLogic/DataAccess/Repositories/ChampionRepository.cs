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

        private const int MINIMUM_NUMBER_OF_GAMES_TO_QUALIFY_AS_CHAMPION = 3;

        private const string CHAMPION_SQL =
            @"SELECT TOP 1 PlayerId,
	             COUNT(PlayerId) AS NumberOfGames,
	             SUM(CASE WHEN PlayerGameResult.GameRank = 1 THEN 1 ELSE 0 END) AS NumberOfWins
             FROM PlayerGameResult
             INNER JOIN PlayedGame
	             ON PlayerGameResult.PlayedGameId = PlayedGame.Id
             INNER JOIN Player
	             ON Player.Id = PlayerGameResult.PlayerId
	             AND Player.Active = 1
             INNER JOIN GameDefinition
	             ON GameDefinition.Id = PlayedGame.GameDefinitionId
             WHERE GameDefinitionId = @GameDefinitionId
             GROUP BY PlayerId
             HAVING COUNT(PlayerId) >= @MinimumNumberOfGames
             ORDER BY NumberOfWins DESC, NumberOfGames DESC";

        public ChampionData GetChampionData(int gameDefinitionId)
        {
            DbRawSqlQuery<ChampionStatistics> championStatisticsData = dataContext.MakeRawSqlQuery<ChampionStatistics>(CHAMPION_SQL,
                new SqlParameter("GameDefinitionId", gameDefinitionId), 
                new SqlParameter("MinimumNumberOfGames", MINIMUM_NUMBER_OF_GAMES_TO_QUALIFY_AS_CHAMPION));

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
