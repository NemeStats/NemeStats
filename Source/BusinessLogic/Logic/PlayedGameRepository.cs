using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace BusinessLogic.Logic
{
    public class PlayedGameRepository : BusinessLogic.Logic.PlayedGameLogic
    {
        private NerdScorekeeperDbContext dbContext;

        public PlayedGameRepository(NerdScorekeeperDbContext context)
        {
            dbContext = context;
        }

        public PlayedGame GetPlayedGameDetails(int playedGameId)
        {
            return dbContext.PlayedGames.Where(playedGame => playedGame.Id == playedGameId).FirstOrDefault();
                                
        }
    }
}
