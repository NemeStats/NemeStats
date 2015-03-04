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
using System.Data.Entity.Infrastructure;
using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
using BusinessLogic.Exceptions;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.Nemeses;

namespace BusinessLogic.Logic.Players
{
    public class PlayerSaver : IPlayerSaver
    {
        private readonly IDataContext dataContext;
        private readonly INemeStatsEventTracker eventTracker;
        private readonly INemesisRecalculator nemesisRecalculator;

        public PlayerSaver(IDataContext dataContext, INemeStatsEventTracker eventTracker, INemesisRecalculator nemesisRecalculator)
        {
            this.dataContext = dataContext;
            this.eventTracker = eventTracker;
            this.nemesisRecalculator = nemesisRecalculator;
        }

        public Player Save(Player player, ApplicationUser currentUser)
        {
            ValidatePlayerIsNotNull(player);
            ValidatePlayerNameIsNotNullOrWhiteSpace(player.Name);
            ValidatePlayerWithThisNameDoesntAlreadyExist(player, currentUser);
            bool alreadyInDatabase = player.AlreadyInDatabase();

            Player newPlayer = dataContext.Save<Player>(player, currentUser);
            dataContext.CommitAllChanges();

            if (!alreadyInDatabase)
            {
                new Task(() => eventTracker.TrackPlayerCreation(currentUser)).Start();
            }else
            {
                if(!player.Active)
                {
                    this.RecalculateNemeses(player, currentUser);
                }
            }

            return newPlayer;
        }

        private void ValidatePlayerWithThisNameDoesntAlreadyExist(Player player, ApplicationUser currentUser)
        {
            if (!player.AlreadyInDatabase())
            {
                Player existingPlayerWithThisName = this.dataContext.GetQueryable<Player>().FirstOrDefault(
                                                                                       p => p.GamingGroupId == currentUser.CurrentGamingGroupId
                                                                                            && p.Name == player.Name);

                if (existingPlayerWithThisName != null)
                {
                    throw new PlayerAlreadyExistsException(existingPlayerWithThisName.Id);
                } 
            }
        }

        private void RecalculateNemeses(Player player, ApplicationUser currentUser)
        {
            List<int> playerIdsToRecalculate = (from thePlayer in this.dataContext.GetQueryable<Player>()
                                                where thePlayer.Active == true
                                                      && thePlayer.Nemesis.NemesisPlayerId == player.Id
                                                select thePlayer.Id).ToList();

            foreach (int playerId in playerIdsToRecalculate)
            {
                this.nemesisRecalculator.RecalculateNemesis(playerId, currentUser);
            }
        }

        private static void ValidatePlayerIsNotNull(Player player)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }
        }

        private static void ValidatePlayerNameIsNotNullOrWhiteSpace(string playerName)
        {
            if (string.IsNullOrWhiteSpace(playerName))
            {
                throw new ArgumentNullException("playerName");
            }
        }
    }
}
