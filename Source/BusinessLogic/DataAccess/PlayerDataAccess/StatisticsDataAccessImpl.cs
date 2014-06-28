using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLogic.Models.Players;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using BusinessLogic.Models;

namespace BusinessLogic.DataAccess.PlayerDataAccess
{
    public class StatisticsDataAccessImpl : StatisticsDataAccess
    {
        private NemeStatsDbContext dbContext;
        private PlayerLogic playerLogic;

        public int MINIMUM_NUMBER_OF_GAMES_TO_BE_A_NEMESIS = 3;

        private static readonly string SQL_GET_WIN_LOSS_GAMES_COUNT = 
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
	               ) AS WonGames
	               GROUP BY PlayerId
            ) AS X
            GROUP BY PlayerId";

        public StatisticsDataAccessImpl(NemeStatsDbContext context, PlayerLogic playerLogic)
        {
            dbContext = context;
            this.playerLogic = playerLogic;
        }

        //TODO refactor this. Might be tricky with anonymous data types. Should I create concrete types?
        public Nemesis GetNemesis(int playerId)
        {
            DbRawSqlQuery<WinLossStatistics> data = dbContext.Database.SqlQuery<WinLossStatistics>(SQL_GET_WIN_LOSS_GAMES_COUNT, 
                new SqlParameter("PlayerId", playerId));

            List<WinLossStatistics> winLossStatistics = data.ToList<WinLossStatistics>();

            var result = (from x in winLossStatistics
                          where x.NumberOfGamesLost > x.NumberOfGamesWon
                          && x.NumberOfGamesLost >= MINIMUM_NUMBER_OF_GAMES_TO_BE_A_NEMESIS
                          select new
                          {
                              NumberOfGamesLost = x.NumberOfGamesLost,
                              LossPercentage = 100 * x.NumberOfGamesLost / (x.NumberOfGamesWon + x.NumberOfGamesLost),
                              NemesisPlayerId = x.VersusPlayerId
                          }).OrderByDescending(nemesisCandidates => nemesisCandidates.LossPercentage).FirstOrDefault();

            if(result == null)
            {
                return new NullNemesis();
            }

            Nemesis nemesis = new Nemesis();
            Player nemesisPlayer = dbContext.Players.Find(result.NemesisPlayerId);
            nemesis.NemesisPlayerId = nemesisPlayer.Id;
            nemesis.NemesisPlayerName = nemesisPlayer.Name;
            nemesis.GamesLostVersusNemesis = result.NumberOfGamesLost;
            nemesis.LossPercentageVersusNemesis = result.LossPercentage;

            return nemesis;
        }
    }
}
