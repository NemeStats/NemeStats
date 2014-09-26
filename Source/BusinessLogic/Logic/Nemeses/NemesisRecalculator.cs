using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models;
using BusinessLogic.Models.Nemeses;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.Nemeses
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

                this.RecalculateNemesis(activePlayer.Id, applicationUser);
            }
        }

        public virtual Nemesis RecalculateNemesis(int playerId, ApplicationUser currentUser)
        {
            Player minionPlayer = dataContext.FindById<Player>(playerId);

            NemesisData nemesisData = playerRepository.GetNemesisData(playerId);

            if (nemesisData is NullNemesisData)
            {
                ClearNemesisId(currentUser, minionPlayer);

                return new NullNemesis();
            }

            Nemesis currentNemesis = dataContext.GetQueryable<Nemesis>()
                                        .Where(nemesis => nemesis.Id == minionPlayer.NemesisId)
                                        .First();

            Nemesis newNemesis = new Nemesis()
            {
                LossPercentage = nemesisData.LossPercentage,
                NumberOfGamesLost = nemesisData.NumberOfGamesLost,
                NemesisPlayerId = nemesisData.NemesisPlayerId,
                MinionPlayerId = playerId
            };

            Nemesis savedNemesis;

            if(currentNemesis.SameNemesis(newNemesis))
            {
                if(currentNemesis.Equals(newNemesis))
                {
                    savedNemesis = newNemesis;
                }else
                {
                    currentNemesis.NumberOfGamesLost = newNemesis.NumberOfGamesLost;
                    currentNemesis.LossPercentage = newNemesis.LossPercentage;
                    savedNemesis = dataContext.Save<Nemesis>(currentNemesis, currentUser);
                    dataContext.CommitAllChanges();
                }
            }else
            {
                savedNemesis = dataContext.Save<Nemesis>(newNemesis, currentUser);
                dataContext.CommitAllChanges();
            }

            minionPlayer.NemesisId = savedNemesis.Id;
            dataContext.Save<Player>(minionPlayer, currentUser);

            return savedNemesis;
        }

        private void ClearNemesisId(ApplicationUser currentUser, Player minionPlayer)
        {
            if (minionPlayer.NemesisId != null)
            {
                minionPlayer.NemesisId = null;
                dataContext.Save<Player>(minionPlayer, currentUser);
            }
        }
    }
}
