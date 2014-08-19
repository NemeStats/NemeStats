using BusinessLogic.DataAccess;
using BusinessLogic.Logic;
using BusinessLogic.Models;
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
    public class EntityFrameworkPlayerRepository : PlayerRepository
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

        private DataContext dataContext;

        public EntityFrameworkPlayerRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public virtual PlayerDetails GetPlayerDetails(int playerID, int numberOfRecentGamesToRetrieve, ApplicationUser currentUser)
        {
            Player returnPlayer = dataContext.FindById<Player>(playerID, currentUser);

            PlayerStatistics playerStatistics = GetPlayerStatistics(playerID, currentUser);

            List<PlayerGameResult> playerGameResults = GetPlayerGameResultsWithPlayedGameAndGameDefinition(playerID, numberOfRecentGamesToRetrieve, currentUser);

            Nemesis nemesis = GetNemesis(playerID, currentUser);

            PlayerDetails playerDetails = new PlayerDetails()
            {
                Active = returnPlayer.Active,
                Id = returnPlayer.Id,
                Name = returnPlayer.Name,
                GamingGroupId = returnPlayer.GamingGroupId,
                PlayerGameResults = playerGameResults,
                PlayerStats = playerStatistics,
                Nemesis = nemesis
            };

            return playerDetails;
        }

        internal virtual List<PlayerGameResult> GetPlayerGameResultsWithPlayedGameAndGameDefinition(
            int playerID, 
            int numberOfRecentGamesToRetrieve, 
            ApplicationUser currentUser)
        {
            List<PlayerGameResult> playerGameResults = dataContext.GetQueryable<PlayerGameResult>(currentUser)
                        .Where(result => result.PlayerId == playerID
                            && result.Player.GamingGroupId == currentUser.CurrentGamingGroupId)
                        .OrderByDescending(result => result.PlayedGame.DatePlayed)
                        .Take(numberOfRecentGamesToRetrieve)
                        .Include(result => result.PlayedGame)
                        .Include(result => result.PlayedGame.GameDefinition)
                        .ToList();
            return playerGameResults;
        }

        public List<Player> GetAllPlayers(bool active, ApplicationUser currentUser)
        {
            return dataContext.GetQueryable<Player>(currentUser).Where(player => player.Active == active
                && player.GamingGroupId == currentUser.CurrentGamingGroupId)
                .OrderBy(player => player.Name)
                .ToList();
        }

        public virtual PlayerStatistics GetPlayerStatistics(int playerId, ApplicationUser currentUser)
        {
            //TODO could hard code the below to get my integration test to pass. 
            //How do I do a 2nd test so that this would need to be fixed?
            //return new PlayerStatistics() { TotalGames = 3 };
            PlayerStatistics playerStatistics = new PlayerStatistics();
            playerStatistics.TotalGames = dataContext.GetQueryable<PlayerGameResult>(currentUser)
                .Count(playerGameResults => playerGameResults.PlayerId == playerId
                && playerGameResults.Player.GamingGroupId == currentUser.CurrentGamingGroupId);
            return playerStatistics;
        }

        //TODO refactor this. Might be tricky with anonymous data types. Should I create concrete types?
        public virtual Nemesis GetNemesis(int playerId, ApplicationUser currentUser)
        {
            //call GetPlayer just to ensure that the requesting user has access
            dataContext.FindById<Player>(playerId, currentUser);
            DbRawSqlQuery<WinLossStatistics> data = dataContext.MakeRawSqlQuery<WinLossStatistics>(SQL_GET_WIN_LOSS_GAMES_COUNT,
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

            if (result == null)
            {
                return new NullNemesis();
            }

            Nemesis nemesis = new Nemesis();
            Player nemesisPlayer = dataContext.GetQueryable<Player>(currentUser).Where(player => player.Id == result.NemesisPlayerId).First();
            nemesis.NemesisPlayerId = nemesisPlayer.Id;
            nemesis.NemesisPlayerName = nemesisPlayer.Name;
            nemesis.GamesLostVersusNemesis = result.NumberOfGamesLost;
            nemesis.LossPercentageVersusNemesis = result.LossPercentage;

            return nemesis;
        }
    }
}
