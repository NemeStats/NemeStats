using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic
{
    public class PlayerLogic
    {
        private NerdScorekeeperDbContext dbContext = null;

        public PlayerLogic(NerdScorekeeperDbContext context)
        {
            dbContext = context;
        }

        public Player GetPlayerDetails(int playerID)
        {
            throw new NotImplementedException();
        }
    }
}
