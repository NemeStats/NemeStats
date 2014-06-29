using BusinessLogic.DataAccess;
using BusinessLogic.Models.Players;
using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;

namespace BusinessLogic.Models
{
    public class PlayerRepository : BusinessLogic.Models.PlayerLogic
    {
        internal const string EXCEPTION_PLAYER_NOT_FOUND = "The specified player does not exist.";
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

        private NemeStatsDbContext dbContext;

        public PlayerRepository(NemeStatsDbContext context)
        {
            dbContext = context;
        }

        public PlayerDetails GetPlayerDetails(int playerID, int numberOfRecentGamesToRetrieve)
        {
            Player returnPlayer = GetPlayer(playerID);

            ValidatePlayerWasFound(returnPlayer);

            PlayerStatistics playerStatistics = GetPlayerStatistics(playerID);

            List<PlayerGameResult> playerGameResults = GetPlayerGameResultsWithPlayedGameAndGameDefinition(playerID, numberOfRecentGamesToRetrieve);

            Nemesis nemesis = GetNemesis(playerID);

            PlayerDetails playerDetails = new PlayerDetails()
            {
                Active = returnPlayer.Active,
                Id = returnPlayer.Id,
                Name = returnPlayer.Name,
                PlayerGameResults = playerGameResults,
                PlayerStats = playerStatistics,
                Nemesis = nemesis
            };

            return playerDetails;
        }

        private static void ValidatePlayerWasFound(Player returnPlayer)
        {
            if (returnPlayer == null)
            {
                throw new ArgumentException(EXCEPTION_PLAYER_NOT_FOUND);
            }
        }

        internal virtual Player GetPlayer(int playerID)
        {
            Player returnPlayer = dbContext.Players
                .Where(player => player.Id == playerID)
                    .FirstOrDefault();
            return returnPlayer;
        }

        internal virtual List<PlayerGameResult> GetPlayerGameResultsWithPlayedGameAndGameDefinition(int playerID, int numberOfRecentGamesToRetrieve)
        {
            List<PlayerGameResult> playerGameResults = dbContext.PlayerGameResults
                        .Where(result => result.PlayerId == playerID)
                        .OrderByDescending(result => result.PlayedGame.DatePlayed)
                        .Take(numberOfRecentGamesToRetrieve)
                        .Include(result => result.PlayedGame)
                        .Include(result => result.PlayedGame.GameDefinition)
                        .ToList();
            return playerGameResults;
        }

        public List<Player> GetAllPlayers(bool active)
        {
            return dbContext.Players.Where(player => player.Active == active).ToList();
        }

        public virtual PlayerStatistics GetPlayerStatistics(int playerId)
        {
            //TODO could hard code the below to get my integration test to pass. 
            //How do I do a 2nd test so that this would need to be fixed?
            //return new PlayerStatistics() { TotalGames = 3 };
            PlayerStatistics playerStatistics = new PlayerStatistics();
            playerStatistics.TotalGames = dbContext.PlayerGameResults.Count(playerGameResults => playerGameResults.PlayerId == playerId);
            return playerStatistics;
        }

        //TODO refactor this. Might be tricky with anonymous data types. Should I create concrete types?
        public virtual Nemesis GetNemesis(int playerId)
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

            if (result == null)
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
