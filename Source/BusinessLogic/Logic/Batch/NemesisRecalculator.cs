using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.Batch
{
    public class NemesisRecalculator : INemesisRecalculator
    {
        private IDataContext dataContext;
        private IPlayerRepository playerRepository;

        public NemesisRecalculator(IDataContext dataContext, IPlayerRepository playerRepository)
        {
            this.dataContext = dataContext;
            this.playerRepository = playerRepository;
        }

        public void RecalculateAllNemeses()
        {
            List<Player> activePlayers = dataContext.GetQueryable<Player>()
                                            .Where(player => player.Active == true)
                                            .ToList();

            ApplicationUser applicationUser = new ApplicationUser();

            foreach(Player activePlayer in activePlayers)
            {
                applicationUser.CurrentGamingGroupId = activePlayer.GamingGroupId;

                playerRepository.RecalculateNemesis(activePlayer.Id, applicationUser);
            }
        }
    }
}
