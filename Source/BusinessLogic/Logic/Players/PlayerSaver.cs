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
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Logic.Champions;

namespace BusinessLogic.Logic.Players
{
    public class PlayerSaver : IPlayerSaver
    {
        private readonly IDataContext _dataContext;
        private readonly INemeStatsEventTracker _eventTracker;
        private readonly INemesisRecalculator _nemesisRecalculator;
        private readonly IPlayerInviter _playerInviter;
        private readonly IChampionRecalculator _championRecalculator;

        public PlayerSaver(IDataContext dataContext, INemeStatsEventTracker eventTracker, INemesisRecalculator nemesisRecalculator, 
            IPlayerInviter playerInviter, IChampionRecalculator championRecalculator)
        {
            _dataContext = dataContext;
            _eventTracker = eventTracker;
            _nemesisRecalculator = nemesisRecalculator;
            _playerInviter = playerInviter;
            _championRecalculator = championRecalculator;
        }
        
        public Player CreatePlayer(CreatePlayerRequest createPlayerRequest, ApplicationUser applicationUser, bool linkCurrentUserToThisPlayer = false)
        {
            ValidateRequestIsNotNull(createPlayerRequest);
            ValidatePlayerNameIsNotNullOrWhiteSpace(createPlayerRequest.Name);
            ValidateRequestedEmailIsntSetAtTheSameTimeAsAttemptingToLinktoCurrentPlayer(createPlayerRequest,
                linkCurrentUserToThisPlayer);
            var gamingGroupId = ValidateAndGetGamingGroupId(createPlayerRequest.GamingGroupId, applicationUser);
            ValidatePlayerDoesntExistWithThisName(createPlayerRequest.Name, gamingGroupId);

            ValidateUserNotAlreadyRegisteredWithThisEmail(
                gamingGroupId, createPlayerRequest.PlayerEmailAddress);

            var newPlayer = new Player
            {
                Name = createPlayerRequest.Name,
                Active = true,
                ApplicationUserId = linkCurrentUserToThisPlayer ? applicationUser.Id : null,
                GamingGroupId = gamingGroupId
            };

            newPlayer = _dataContext.Save(newPlayer, applicationUser);

            if (!string.IsNullOrWhiteSpace(createPlayerRequest.PlayerEmailAddress))
            {
                var playerInvitation = new PlayerInvitation
                {
                    EmailSubject = $"NemeStats Invitation from {applicationUser.UserName}",
                    InvitedPlayerEmail = createPlayerRequest.PlayerEmailAddress,
                    InvitedPlayerId = newPlayer.Id,
                    GamingGroupId = gamingGroupId
                };
                _playerInviter.InvitePlayer(playerInvitation, applicationUser);
            }

            _dataContext.CommitAllChanges();

            new Task(() => _eventTracker.TrackPlayerCreation(applicationUser)).Start();

            return newPlayer;
        }

        private int ValidateAndGetGamingGroupId(int? requestedGamingGroupId, ApplicationUser applicationUser)
        {
            if (requestedGamingGroupId.HasValue)
            {
                return requestedGamingGroupId.Value;
            }

            if (applicationUser.CurrentGamingGroupId.HasValue)
            {
                return applicationUser.CurrentGamingGroupId.Value;
            }

            throw new NoValidGamingGroupException(applicationUser.Id);
        }

        private void ValidateRequestIsNotNull(CreatePlayerRequest createPlayerRequest)
        {
            if (createPlayerRequest == null)
            {
                throw new ArgumentNullException(nameof(createPlayerRequest));
            }
        }

        private void ValidateRequestedEmailIsntSetAtTheSameTimeAsAttemptingToLinktoCurrentPlayer(CreatePlayerRequest createPlayerRequest, bool linkCurrentUserToThisPlayer)
        {
            if (!string.IsNullOrWhiteSpace(createPlayerRequest.PlayerEmailAddress)
                && linkCurrentUserToThisPlayer)
            {
                throw new ArgumentException(
                    "You cannot specify an email address for the new Player while simultaneously requesting to associate the Player with the current user.");
            }
        }

        private void ValidatePlayerDoesntExistWithThisName(string playerName, int gamingGroupId)
        {
            var existingPlayerWithThisName = _dataContext.GetQueryable<Player>()
                .FirstOrDefault(p => p.GamingGroupId == gamingGroupId
                                     && p.Name == playerName);

            if (existingPlayerWithThisName != null)
            {
                throw new PlayerAlreadyExistsException(playerName, existingPlayerWithThisName.Id);
            }
        }

        private void ValidateUserNotAlreadyRegisteredWithThisEmail(int gamingGroupId, string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                return;
            }
            var existingPlayer = _dataContext.GetQueryable<Player>()
                .FirstOrDefault(p => p.GamingGroupId == gamingGroupId
                                     && p.ApplicationUserId != null 
                                     && p.User.Email == emailAddress);

            if (existingPlayer != null)
            {
                throw new PlayerWithThisEmailAlreadyExistsException(emailAddress, existingPlayer.Name, existingPlayer.Id);
            }
        }

        public virtual void UpdatePlayer(UpdatePlayerRequest updatePlayerRequest, ApplicationUser applicationUser)
        {
            var player = _dataContext.FindById<Player>(updatePlayerRequest.PlayerId);

            var somethingChanged = false;

            if (updatePlayerRequest.Active.HasValue && updatePlayerRequest.Active != player.Active)
            {
                player.Active = updatePlayerRequest.Active.Value;

                somethingChanged = true;
            }

            if (!string.IsNullOrWhiteSpace(updatePlayerRequest.Name) && updatePlayerRequest.Name.Trim() != player.Name)
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
            ValidatePlayerWithThisNameDoesntAlreadyExist(player);
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

                    RecalculateChampions(player, applicationUser);
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

        private void ValidatePlayerWithThisNameDoesntAlreadyExist(Player player)
        {
            var playerWithNameAlreadyExists = _dataContext.GetQueryable<Player>()
                .FirstOrDefault(x => x.Name == player.Name 
                          && x.GamingGroupId == player.GamingGroupId
                          && x.Id != player.Id);

            if (playerWithNameAlreadyExists != null)
            {
                throw new PlayerAlreadyExistsException(player.Name, playerWithNameAlreadyExists.Id);
            }
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

        private void RecalculateChampions(Player player, ApplicationUser applicationUser)
        {
            var championedGameDefinitionIds = _dataContext.GetQueryable<GameDefinition>()
                .Where(x => x.Champion.PlayerId == player.Id).Select(x => x.Id).ToList();

            championedGameDefinitionIds.ForEach(gameDefinitionId =>
                _championRecalculator.RecalculateChampion(gameDefinitionId, applicationUser, _dataContext));
        }
    }
}
