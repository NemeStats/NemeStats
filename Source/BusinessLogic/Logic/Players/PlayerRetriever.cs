using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Logic.Players
{
    public class PlayerRetriever : BusinessLogic.Logic.Players.IPlayerRetriever
    {
        private IDataContext dataContext;

        public PlayerRetriever(DataAccess.IDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public List<Player> GetAllPlayers(int gamingGroupId)
        {
            return dataContext.GetQueryable<Player>().Where(
                player => player.GamingGroupId == gamingGroupId
                    && player.Active)
                .OrderBy(player => player.Name)
                .ToList();
        }
    }
}
