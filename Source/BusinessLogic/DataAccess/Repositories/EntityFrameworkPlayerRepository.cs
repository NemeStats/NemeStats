using BusinessLogic.DataAccess;
using BusinessLogic.Logic;
using BusinessLogic.Models;
using BusinessLogic.Models.Nemeses;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;

namespace BusinessLogic.DataAccess.Repositories
{
    public class EntityFrameworkPlayerRepository : IPlayerRepository
    {
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

        private IDataContext dataContext;

        public EntityFrameworkPlayerRepository(IDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public virtual PlayerDetails GetPlayerDetails(int playerID, int numberOfRecentGamesToRetrieve)
        {
            Player returnPlayer = dataContext.FindById<Player>(playerID);

            PlayerStatistics playerStatistics = GetPlayerStatistics(playerID);

            List<PlayerGameResult> playerGameResults = GetPlayerGameResultsWithPlayedGameAndGameDefinition(playerID, numberOfRecentGamesToRetrieve);

            Nemesis nemesis = RetrieveNemesis(returnPlayer.NemesisId);

            PlayerDetails playerDetails = new PlayerDetails()
            {
                Active = returnPlayer.Active,
                Id = returnPlayer.Id,
                Name = returnPlayer.Name,
                GamingGroupId = returnPlayer.GamingGroupId,
                PlayerGameResults = playerGameResults,
                PlayerStats = playerStatistics,
                PlayerNemesis = nemesis
            };

            return playerDetails;
        }

        internal virtual List<PlayerGameResult> GetPlayerGameResultsWithPlayedGameAndGameDefinition(
            int playerID, 
            int numberOfRecentGamesToRetrieve)
        {
            List<PlayerGameResult> playerGameResults = dataContext.GetQueryable<PlayerGameResult>()
                        .Where(result => result.PlayerId == playerID)
                        .OrderByDescending(result => result.PlayedGame.DatePlayed)
                        .Take(numberOfRecentGamesToRetrieve)
                        .Include(result => result.PlayedGame)
                        .Include(result => result.PlayedGame.GameDefinition)
                        .ToList();
            return playerGameResults;
        }

        public virtual PlayerStatistics GetPlayerStatistics(int playerId)
        {
            PlayerStatistics playerStatistics = new PlayerStatistics();
            playerStatistics.TotalGames = dataContext.GetQueryable<PlayerGameResult>()
                .Count(playerGameResults => playerGameResults.PlayerId == playerId);

            int? totalPoints = dataContext.GetQueryable<PlayerGameResult>()
                .Where(result => result.PlayerId == playerId)
                //had to cast to handle the case where there is no data:
                //http://stackoverflow.com/questions/6864311/the-cast-to-value-type-int32-failed-because-the-materialized-value-is-null
                .Sum(playerGameResults => (int?)playerGameResults.GordonPoints) ?? 0;

            if(totalPoints.HasValue)
            {
                playerStatistics.TotalPoints = totalPoints.Value;
            }

            //had to cast to handle the case where there is no data:
            //http://stackoverflow.com/questions/6864311/the-cast-to-value-type-int32-failed-because-the-materialized-value-is-null
            playerStatistics.AveragePlayersPerGame = (float?)dataContext.GetQueryable<PlayedGame>()
                .Where(playedGame => playedGame.PlayerGameResults.Any(result => result.PlayerId == playerId))
                    .Average(game => (int?)game.NumberOfPlayers) ?? 0F;

            return playerStatistics;
        }

        private Nemesis RetrieveNemesis(int? nemesisId)
        {
            Nemesis nemesis;
            if (nemesisId.HasValue)
            {
                nemesis = dataContext.FindById<Nemesis>(nemesisId.Value);
                nemesis.NemesisPlayer = dataContext.FindById<Player>(nemesis.NemesisPlayerId);
            }
            else
            {
                nemesis = new NullNemesis();
            }
            return nemesis;
        }

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
    }
}
