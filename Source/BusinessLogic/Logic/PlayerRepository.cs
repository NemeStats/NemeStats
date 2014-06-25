using BusinessLogic.DataAccess;
using BusinessLogic.Models.Players;
using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    public class PlayerRepository : BusinessLogic.Models.PlayerLogic
    {
        private NemeStatsDbContext dbContext = null;

        public PlayerRepository(NemeStatsDbContext context)
        {
            dbContext = context;
        }

        public PlayerDetails GetPlayerDetails(int playerID, int numberOfRecentGamesToRetrieve)
        {
            Player returnPlayer = GetPlayer(playerID);

            PlayerDetails playerDetails = null;
            
            if(returnPlayer != null)
            {
                PlayerStatistics playerStatistics = GetPlayerStatistics(playerID);

                List<PlayerGameResult> playerGameResults = GetPlayerGameResultsWithPlayedGameAndGameDefinition(playerID, numberOfRecentGamesToRetrieve);

                playerDetails = new PlayerDetails()
                {
                    Active = returnPlayer.Active,
                    Id = returnPlayer.Id,
                    Name = returnPlayer.Name,
                    PlayerGameResults = playerGameResults,
                    PlayerStats = playerStatistics
                };
            }
            
            return playerDetails;
        }

        private Player GetPlayer(int playerID)
        {
            Player returnPlayer = dbContext.Players
                .Where(player => player.Id == playerID)
                    .FirstOrDefault();
            return returnPlayer;
        }

        private List<PlayerGameResult> GetPlayerGameResultsWithPlayedGameAndGameDefinition(int playerID, int numberOfRecentGamesToRetrieve)
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

        public PlayerStatistics GetPlayerStatistics(int playerId)
        {
            //TODO could hard code the below to get my integration test to pass. 
            //How do I do a 2nd test so that this would need to be fixed?
            //return new PlayerStatistics() { TotalGames = 3 };
            PlayerStatistics playerStatistics = new PlayerStatistics();
            playerStatistics.TotalGames = dbContext.PlayerGameResults.Count(playerGameResults => playerGameResults.PlayerId == playerId);
            return playerStatistics;
        }
    }
}
