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
using BusinessLogic.EventTracking;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public virtual Player Save(Player player, ApplicationUser applicationUser)
        {
            ValidatePlayerIsNotNull(player);
            ValidatePlayerNameIsNotNullOrWhiteSpace(player.Name);
            ValidatePlayerWithThisNameDoesntAlreadyExist(player, applicationUser);
            var alreadyInDatabase = player.AlreadyInDatabase();

            var newPlayer = dataContext.Save(player, applicationUser);
            dataContext.CommitAllChanges();

            if (!alreadyInDatabase)
            {
                new Task(() => eventTracker.TrackPlayerCreation(applicationUser)).Start();
            }else
            {
                if(!player.Active)
                {
                    this.RecalculateNemeses(player, applicationUser);
                }
            }

            return newPlayer;
        }

        public Player CreatePlayer(CreatePlayerRequest createPlayerRequest, ApplicationUser applicationUser)
        {
            if (createPlayerRequest == null)
            {
                throw new ArgumentNullException(nameof(createPlayerRequest));
            }
            ValidatePlayerNameIsNotNullOrWhiteSpace(createPlayerRequest.Name);
            ThrowPlayerAlreadyExistsExceptionIfPlayerExistsWithThisName(createPlayerRequest.Name, applicationUser);
            var newPlayer = new Player
            {
                Name = createPlayerRequest.Name,
                Active = true
            };

            newPlayer = dataContext.Save(newPlayer, applicationUser);
            dataContext.CommitAllChanges();

            new Task(() => eventTracker.TrackPlayerCreation(applicationUser)).Start();

            return newPlayer;
        }

        private void ValidatePlayerWithThisNameDoesntAlreadyExist(Player player, ApplicationUser currentUser)
        {
            if (player.AlreadyInDatabase())
            {
                return;
            }
            ThrowPlayerAlreadyExistsExceptionIfPlayerExistsWithThisName(player.Name, currentUser);
        }

        private void ThrowPlayerAlreadyExistsExceptionIfPlayerExistsWithThisName(string playerName, ApplicationUser currentUser)
        {
            var existingPlayerWithThisName = this.dataContext.GetQueryable<Player>().FirstOrDefault(
                                                                                                       p => p.GamingGroupId == currentUser.CurrentGamingGroupId
                                                                                                            && p.Name == playerName);

            if (existingPlayerWithThisName != null)
            {
                throw new PlayerAlreadyExistsException(playerName, existingPlayerWithThisName.Id);
            }
        }

        private void RecalculateNemeses(Player player, ApplicationUser currentUser)
        {
            var playerIdsToRecalculate = (from thePlayer in this.dataContext.GetQueryable<Player>()
                                                where thePlayer.Active
                                                      && thePlayer.Nemesis.NemesisPlayerId == player.Id
                                                select thePlayer.Id).ToList();

            foreach (var playerId in playerIdsToRecalculate)
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

        public virtual void UpdatePlayer(UpdatePlayerRequest updatePlayerRequest, ApplicationUser applicationUser)
        {
            var player = dataContext.FindById<Player>(updatePlayerRequest.PlayerId);

            var somethingChanged = false;

            if (updatePlayerRequest.Active.HasValue)
            {
                player.Active = updatePlayerRequest.Active.Value;

                somethingChanged = true;
            }

            if (!string.IsNullOrWhiteSpace(updatePlayerRequest.Name))
            {
                player.Name = updatePlayerRequest.Name.Trim();

                somethingChanged = true;
            }

            if (somethingChanged)
            {
                Save(player, applicationUser);
            }
        }
    }
}
