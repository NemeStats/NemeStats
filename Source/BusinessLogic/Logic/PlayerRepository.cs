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
            Player returnPlayer = dbContext.Players
                .Where(player => player.Id == playerID)
                .Include(player => player.PlayerGameResults
                    .Select(playerGameResult => playerGameResult.PlayedGame)
                        .Select(playedGame => playedGame.GameDefinition))
                        .FirstOrDefault();

            PlayerDetails playerDetails = null;
            
            if(returnPlayer != null)
            {
                PlayerStatistics playerStatistics = GetPlayerStatistics(playerID);

                playerDetails = new PlayerDetails()
                {
                    Active = returnPlayer.Active,
                    Id = returnPlayer.Id,
                    Name = returnPlayer.Name,
                    //TODO this should happen in the query above rather than after the query. Need help with this though.
                    PlayerGameResults = returnPlayer.PlayerGameResults.Take(numberOfRecentGamesToRetrieve).ToList(),
                    PlayerStats = playerStatistics
                };
            }
            
            return playerDetails;
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
