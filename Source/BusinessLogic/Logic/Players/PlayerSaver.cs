﻿#region LICENSE
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
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.Players
{
    public class PlayerSaver : IPlayerSaver
    {
        private readonly IDataContext _dataContext;
        private readonly INemeStatsEventTracker _eventTracker;
        private readonly INemesisRecalculator _nemesisRecalculator;

        public PlayerSaver(IDataContext dataContext, INemeStatsEventTracker eventTracker, INemesisRecalculator nemesisRecalculator)
        {
            _dataContext = dataContext;
            _eventTracker = eventTracker;
            _nemesisRecalculator = nemesisRecalculator;
        }
        
        public Player CreatePlayer(CreatePlayerRequest createPlayerRequest, ApplicationUser applicationUser, bool linkCurrentUserToThisPlayer = false)
        {
            if (createPlayerRequest == null)
            {
                throw new ArgumentNullException(nameof(createPlayerRequest));
            }
            ValidatePlayerNameIsNotNullOrWhiteSpace(createPlayerRequest.Name);
            if (!applicationUser.CurrentGamingGroupId.HasValue)
            {
                throw new UserHasNoGamingGroupException(applicationUser.Id);
            }
            int gamingGroupId = createPlayerRequest.GamingGroupId ?? applicationUser.CurrentGamingGroupId.Value;
            ThrowPlayerAlreadyExistsExceptionIfPlayerExistsWithThisName(createPlayerRequest.Name, gamingGroupId);

            var newPlayer = new Player
            {
                Name = createPlayerRequest.Name,
                Active = true,
                ApplicationUserId = linkCurrentUserToThisPlayer ? applicationUser.Id : null,
                GamingGroupId = gamingGroupId
            };

            newPlayer = _dataContext.Save(newPlayer, applicationUser);
            _dataContext.CommitAllChanges();

            new Task(() => _eventTracker.TrackPlayerCreation(applicationUser)).Start();

            return newPlayer;
        }

        private void ThrowPlayerAlreadyExistsExceptionIfPlayerExistsWithThisName(string playerName, int gamingGroupId)
        {
            var existingPlayerWithThisName = _dataContext.GetQueryable<Player>().FirstOrDefault(
                                                                                                       p => p.GamingGroupId == gamingGroupId
                                                                                                            && p.Name == playerName);

            if (existingPlayerWithThisName != null)
            {
                throw new PlayerAlreadyExistsException(playerName, existingPlayerWithThisName.Id);
            }
        }

        public virtual void UpdatePlayer(UpdatePlayerRequest updatePlayerRequest, ApplicationUser applicationUser)
        {
            var player = _dataContext.FindById<Player>(updatePlayerRequest.PlayerId);

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

        internal virtual Player Save(Player player, ApplicationUser applicationUser)
        {
            ValidatePlayerIsNotNull(player);
            ValidatePlayerNameIsNotNullOrWhiteSpace(player.Name);
            ValidateUserHasGamingGroup(applicationUser);
            ValidatePlayerWithThisNameDoesntAlreadyExist(player, applicationUser.CurrentGamingGroupId.Value);
            var alreadyInDatabase = player.AlreadyInDatabase();

            var newPlayer = _dataContext.Save(player, applicationUser);
            _dataContext.CommitAllChanges();

            if (!alreadyInDatabase)
            {
                new Task(() => _eventTracker.TrackPlayerCreation(applicationUser)).Start();
            }
            else
            {
                if (!player.Active)
                {
                    RecalculateNemeses(player, applicationUser);
                }
            }

            return newPlayer;
        }

        private static void ValidatePlayerIsNotNull(Player player)
        {
            if (player == null)
            {
                throw new ArgumentNullException(nameof(player));
            }
        }

        private static void ValidatePlayerNameIsNotNullOrWhiteSpace(string playerName)
        {
            if (string.IsNullOrWhiteSpace(playerName))
            {
                throw new ArgumentNullException(nameof(playerName));
            }
        }

        private static void ValidateUserHasGamingGroup(ApplicationUser applicationUser)
        {
            if (!applicationUser.CurrentGamingGroupId.HasValue)
            {
                throw new UserHasNoGamingGroupException(applicationUser.Id);
            }
        }

        private void ValidatePlayerWithThisNameDoesntAlreadyExist(Player player, int gamingGroupId)
        {
            if (player.AlreadyInDatabase())
            {
                return;
            }
            ThrowPlayerAlreadyExistsExceptionIfPlayerExistsWithThisName(player.Name, gamingGroupId);
        }

        private void RecalculateNemeses(Player player, ApplicationUser currentUser)
        {
            var playerIdsToRecalculate = (from thePlayer in _dataContext.GetQueryable<Player>()
                                          where thePlayer.Active
                                                && thePlayer.Nemesis.NemesisPlayerId == player.Id
                                          select thePlayer.Id).ToList();

            foreach (var playerId in playerIdsToRecalculate)
            {
                _nemesisRecalculator.RecalculateNemesis(playerId, currentUser, _dataContext);
            }
        }
    }
}
