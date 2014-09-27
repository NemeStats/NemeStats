using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace BusinessLogic.Logic.Players
{
    public class PlayerRetriever : BusinessLogic.Logic.Players.IPlayerRetriever
    {
        private IDataContext dataContext;

        public PlayerRetriever(DataAccess.IDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        internal IQueryable<Player> GetAllPlayersInGamingGroupQueryable(int gamingGroupId)
        {
            return dataContext.GetQueryable<Player>().Where(
               player => player.GamingGroupId == gamingGroupId
                   && player.Active);
        }

        public List<Player> GetAllPlayers(int gamingGroupId)
        {
            return GetAllPlayersInGamingGroupQueryable(gamingGroupId)
                .OrderBy(player => player.Name)
                .ToList();
        }

        public List<Player> GetAllPlayersWithNemesisInfo(int gamingGroupId)
        {
            return GetAllPlayersInGamingGroupQueryable(gamingGroupId)
                                        .Include(player => player.Nemesis)
                                        .Include(player => player.Nemesis.NemesisPlayer)
                                        .OrderBy(player => player.Name)
                                        .ToList();
        }
    }
}
