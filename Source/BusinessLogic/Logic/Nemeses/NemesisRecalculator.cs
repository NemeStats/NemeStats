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
using System.Linq;

namespace BusinessLogic.Logic.Nemeses
{
    public class NemesisRecalculator : INemesisRecalculator
    {
        private readonly IDataContext _dataContext;
        private readonly IPlayerRepository _playerRepository;

        public NemesisRecalculator(IDataContext dataContext, IPlayerRepository playerRepository)
        {
            this._dataContext = dataContext;
            this._playerRepository = playerRepository;
        }

        public void RecalculateAllNemeses()
        {
            var activePlayers = _dataContext.GetQueryable<Player>()
                                            .Where(player => player.Active)
                                            .ToList();

            var applicationUser = new ApplicationUser();

            foreach(var activePlayer in activePlayers)
            {
                applicationUser.CurrentGamingGroupId = activePlayer.GamingGroupId;

                this.RecalculateNemesis(activePlayer.Id, applicationUser, _dataContext);
            }
        }

        public virtual Nemesis RecalculateNemesis(int playerId, ApplicationUser currentUser, IDataContext dataContext)
        {
            var minionPlayer = dataContext.FindById<Player>(playerId);

            var nemesisData = _playerRepository.GetNemesisData(playerId, dataContext);

            if (nemesisData is NullNemesisData)
            {
                ClearNemesisId(currentUser, minionPlayer);

                return new NullNemesis();
            }

            var existingNemesis = _dataContext
                                        .GetQueryable<Nemesis>()
                                        .FirstOrDefault(nemesis => nemesis.Id == minionPlayer.NemesisId);

            var newNemesis = new Nemesis()
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
                savedNemesis = _dataContext.Save<Nemesis>(newNemesis, currentUser);
                _dataContext.CommitAllChanges();
                minionPlayer.PreviousNemesisId = minionPlayer.NemesisId;
                minionPlayer.NemesisId = savedNemesis.Id;
                _dataContext.Save<Player>(minionPlayer, currentUser);
            }

            return savedNemesis;
        }

        private Nemesis UpdateExistingNemesisIfNeeded(ApplicationUser currentUser, Nemesis existingNemesis, Nemesis newNemesis)
        {
            if (!newNemesis.Equals(existingNemesis))
            {
                existingNemesis.NumberOfGamesLost = newNemesis.NumberOfGamesLost;
                existingNemesis.LossPercentage = newNemesis.LossPercentage;
                var returnNemesis = _dataContext.Save<Nemesis>(existingNemesis, currentUser);
                _dataContext.CommitAllChanges();
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
                _dataContext.Save<Player>(minionPlayer, currentUser);
            }
        }
    }
}
