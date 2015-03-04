#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models;
using BusinessLogic.Models.Nemeses;
using BusinessLogic.Models.User;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Logic.Nemeses
{
    public class NemesisRecalculator : INemesisRecalculator
    {
        private readonly IDataContext dataContext;
        private readonly IPlayerRepository playerRepository;

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

            Nemesis existingNemesis = dataContext.GetQueryable<Nemesis>()
                                        .Where(nemesis => nemesis.Id == minionPlayer.NemesisId)
                                        .FirstOrDefault();

            Nemesis newNemesis = new Nemesis()
            {
                LossPercentage = nemesisData.LossPercentage,
                NumberOfGamesLost = nemesisData.NumberOfGamesLost,
                NemesisPlayerId = nemesisData.NemesisPlayerId,
                MinionPlayerId = playerId
            };

            Nemesis savedNemesis;

            if (newNemesis.SameNemesis(existingNemesis))
            {
                savedNemesis = UpdateExistingNemesisIfNeeded(currentUser, existingNemesis, newNemesis);
            }else
            {
                savedNemesis = dataContext.Save<Nemesis>(newNemesis, currentUser);
                dataContext.CommitAllChanges();
                minionPlayer.PreviousNemesisId = minionPlayer.NemesisId;
                minionPlayer.NemesisId = savedNemesis.Id;
                dataContext.Save<Player>(minionPlayer, currentUser);
            }

            return savedNemesis;
        }

        private Nemesis UpdateExistingNemesisIfNeeded(ApplicationUser currentUser, Nemesis existingNemesis, Nemesis newNemesis)
        {
            if (!newNemesis.Equals(existingNemesis))
            {
                existingNemesis.NumberOfGamesLost = newNemesis.NumberOfGamesLost;
                existingNemesis.LossPercentage = newNemesis.LossPercentage;
                Nemesis returnNemesis = dataContext.Save<Nemesis>(existingNemesis, currentUser);
                dataContext.CommitAllChanges();
                return returnNemesis;
            }
            return existingNemesis;
        }

        private void ClearNemesisId(ApplicationUser currentUser, Player minionPlayer)
        {
            if (minionPlayer.NemesisId != null)
            {
                minionPlayer.PreviousNemesisId = minionPlayer.NemesisId;
                minionPlayer.NemesisId = null;
                dataContext.Save<Player>(minionPlayer, currentUser);
            }
        }
    }
}
