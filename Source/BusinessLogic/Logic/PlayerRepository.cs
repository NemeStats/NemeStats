using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Statistics;
using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic
{
    public class PlayerRepository : BusinessLogic.Logic.PlayerLogic
    {
        private NemeStatsDbContext dbContext = null;

        public PlayerRepository(NemeStatsDbContext context)
        {
            dbContext = context;
        }

        public Player GetPlayerDetails(int playerID)
        {
            return dbContext.Players
                .Where(player => player.Id == playerID)
                .FirstOrDefault();
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
