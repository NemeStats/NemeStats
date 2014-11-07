using BusinessLogic.Models;
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
        public int MINIMUM_NUMBER_OF_GAMES_TO_BE_A_NEMESIS = 3;

        private readonly IDataContext dataContext;

        public EntityFrameworkPlayerRepository(IDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        private const string SQL_GET_WIN_LOSS_GAMES_COUNT =
            @"SELECT SUM(NumberOfGamesLost) AS NumberOfGamesLost, SUM(NumberOfGamesWon) AS NumberOfGamesWon, PlayerId as VersusPlayerId
            FROM
            (
	            SELECT COUNT(*) AS NumberOfGamesLost, 0 AS NumberOfGamesWon, PlayerId
	              FROM
	              (
		              SELECT OtherResults.PlayerId
		              ,OtherResults.PlayedGameId
		              ,OtherResults.GameRank
		              , PlayedGame.GameDefinitionId
		              FROM PlayerGameResult Inner Join PlayedGame ON PlayergameResult.PlayedGameId = PlayedGame.Id
		              INNER JOIN PlayerGameResult OtherResults ON PlayedGame.Id = OtherResults.PlayedGameId
		              WHERE PlayerGameResult.PlayerId = @PlayerId
		              AND OtherResults.GameRank < 
			            (
				            SELECT GameRank FROM PlayerGameResult PGR 
				            WHERE PGR.PlayedGameId = OtherResults.PlayedGameId 
				            AND PGR.PlayerId = @PlayerId
			            )
                        AND EXISTS (SELECT 1 FROM Player WHERE Player.Id = OtherResults.PlayerId AND Player.Active = 1)
	               ) AS LostGames
	               GROUP BY PlayerId
	               UNION
	               SELECT 0 AS NumberOfgamesLost, COUNT(*) AS NumberOfGamesWon, PlayerId
	              FROM
	              (
		              SELECT OtherResults.PlayerId
		              ,OtherResults.PlayedGameId
		              ,OtherResults.GameRank
		              , PlayedGame.GameDefinitionId
		              FROM PlayerGameResult Inner Join PlayedGame ON PlayergameResult.PlayedGameId = PlayedGame.Id
		              INNER JOIN PlayerGameResult OtherResults ON PlayedGame.Id = OtherResults.PlayedGameId
		              WHERE PlayerGameResult.PlayerId = @PlayerId
		              AND OtherResults.GameRank > 
			            (
				            SELECT GameRank FROM PlayerGameResult PGR 
				            WHERE PGR.PlayedGameId = OtherResults.PlayedGameId 
				            AND PGR.PlayerId = @PlayerId
			            ) 
                        AND EXISTS (SELECT 1 FROM Player WHERE Player.Id = OtherResults.PlayerId AND Player.Active = 1)
	               ) AS WonGames
	               GROUP BY PlayerId
            ) AS X
            GROUP BY PlayerId";

        public NemesisData GetNemesisData(int playerId)
        {
            DbRawSqlQuery<WinLossStatistics> data = dataContext.MakeRawSqlQuery<WinLossStatistics>(SQL_GET_WIN_LOSS_GAMES_COUNT,
                new SqlParameter("PlayerId", playerId));

            List<WinLossStatistics> winLossStatistics = data.ToList<WinLossStatistics>();

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
	      ,SUM(CASE WHEN PGR.GameRank = 1 THEN 1 ELSE 0 END) AS GamesWon
	      ,COUNT(*) AS GamesPlayed
          FROM [dbo].[GameDefinition] GD 
          INNER JOIN PlayedGame PG ON GD.ID = PG.GameDefinitionID
          INNER JOIN PlayerGameResult PGR ON PG.ID = PGR.PlayedGameId
          WHERE PGR.PlayerId = @PlayerId
          GROUP BY GD.[Id], GD.[Name]
          ORDER BY GamesPlayed DESC, GameDefinitionName";

        public List<PlayerGameSummary> GetPlayerGameSummaries(int playerId)
        {
            DbRawSqlQuery<PlayerGameSummary> data = dataContext.MakeRawSqlQuery<PlayerGameSummary>(SQL_GET_PLAYER_GAME_SUMMARY_INFO,
                new SqlParameter("PlayerId", playerId));

            return data.ToList<PlayerGameSummary>();
        }
    }
}
