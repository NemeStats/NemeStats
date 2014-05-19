using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Logic
{
    public class CompletedGame
    {
        NerdScorekeeperDbContext context = null;

        public CompletedGame(NerdScorekeeperDbContext dbContext)
        {
            context = dbContext;
        }

        public PlayedGame CreatePlayedGame(PlayedGame playedGame)
        {
            if(playedGame.Id != 0)
            {
                throw new ArgumentException("Cannot pass an existing PlayedGame to this method.");
            }
            return null;
        }
    }
}
