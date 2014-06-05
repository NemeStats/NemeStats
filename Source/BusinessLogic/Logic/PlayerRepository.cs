using BusinessLogic.DataAccess;
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
    }
}
