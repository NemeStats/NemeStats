using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Logic.Players
{
    public class PlayerRetrieverImpl : BusinessLogic.Logic.Players.PlayerRetriever
    {
        private DataContext dataContext;

        public PlayerRetrieverImpl(DataAccess.DataContext dataContext)
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
