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
        private const double MINIMUM_WIN_RATIO_TO_BE_CHAMPION = 0.5;

        private readonly IDataContext dataContext;

        public ChampionRepository(IDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        private const string CHAMPION_SQL =
            @"SELECT TOP 1 PlayerId, COUNT(PlayerId) AS NumberOfWins
              FROM PlayerGameResult
              INNER JOIN PlayedGame
                  ON PlayerGameResult.PlayedGameId = PlayedGame.Id
              INNER JOIN Player
                  ON Player.Id = PlayerGameResult.PlayerId
                  AND Player.Active = 1
              INNER JOIN GameDefinition
                  ON GameDefinition.Id = PlayedGame.GameDefinitionId
              WHERE GameRank = 1
              AND GameDefinitionId = @GameDefinitionId
              GROUP BY PlayerId
              ORDER BY NumberOfWins DESC";

        private const string NUMBER_OF_GAMES_FOR_GAME_DEFINITION_SQL =
            @"SELECT COUNT(Id) AS NumberOfGames
              FROM PlayedGame
              WHERE GameDefinitionId = @GameDefinitionId";

        public ChampionData GetChampionData(int gameDefinitionId)
        {
            DbRawSqlQuery<ChampionStatistics> championStatisticsData = dataContext.MakeRawSqlQuery<ChampionStatistics>(CHAMPION_SQL,
                new SqlParameter("GameDefinitionId", gameDefinitionId));

            DbRawSqlQuery<int> numberOfGamesData =
                dataContext.MakeRawSqlQuery<int>(NUMBER_OF_GAMES_FOR_GAME_DEFINITION_SQL,
                    new SqlParameter("GameDefinitionId", gameDefinitionId));

            List<ChampionStatistics> championStatistics = championStatisticsData.ToList();
            int numberOfGames = numberOfGamesData.First();

            ChampionData championData = (from x in championStatistics
                where (double) x.NumberOfWins/numberOfGames >= MINIMUM_WIN_RATIO_TO_BE_CHAMPION
                select new ChampionData
                {
                    PlayerId = x.PlayerId,
                    WinPercentage = 100 * x.NumberOfWins / numberOfGames,
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
