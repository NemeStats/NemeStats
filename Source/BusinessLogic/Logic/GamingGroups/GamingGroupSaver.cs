#region LICENSE

// NemeStats is a free website for tracking the results of board games. Copyright (C) 2015 Jacob Gordon
// 
// This program is free software: you can redistribute it and/or modify it under the terms of the
// GNU General Public License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
// even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with this program. If
// not, see <http://www.gnu.org/licenses/>

#endregion LICENSE

using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.User;
using System;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models.Players;

namespace BusinessLogic.Logic.GamingGroups
{
    public class GamingGroupSaver : IGamingGroupSaver
    {
        internal const string EXCEPTION_MESSAGE_GAMING_GROUP_NAME_CANNOT_BE_NULL_OR_BLANK = "GamingGroup name cannot be null or blank";
        internal const string EXCEPTION_MESSAGE_PLAYER_NAMES_CANNOT_BE_NULL = "gamingGroupQuickStart.NewPlayerNames cannot be null.";
        internal const string EXCEPTION_MESSAGE_GAME_DEFINITION_NAMES_CANNOT_BE_NULL = "gamingGroupQuickStart.NewGameDefinitionNames cannot be null.";

        private readonly IDataContext _dataContext;
        private readonly IPlayerSaver _playerSaver;
        private readonly INemeStatsEventTracker _eventTracker;
        private readonly IGamingGroupContextSwitcher _gamingGroupContextSwitcher;

        public GamingGroupSaver(
            IDataContext dataContext,
            INemeStatsEventTracker eventTracker,
            IPlayerSaver playerSaver, 
            IGamingGroupContextSwitcher gamingGroupContextSwitcher)
        {
            _dataContext = dataContext;
            _eventTracker = eventTracker;
            _playerSaver = playerSaver;
            _gamingGroupContextSwitcher = gamingGroupContextSwitcher;
        }

        public virtual NewlyCreatedGamingGroupResult CreateNewGamingGroup(
            string gamingGroupName,
            TransactionSource registrationSource,
            ApplicationUser currentUser)
        {
            ValidateGamingGroupName(gamingGroupName);

            var gamingGroup = new GamingGroup()
            {
                OwningUserId = currentUser.Id,
                Name = gamingGroupName
            };

            var newlyCreatedGamingGroupResult = new NewlyCreatedGamingGroupResult();
            var newGamingGroup = _dataContext.Save<GamingGroup>(gamingGroup, currentUser);
            newlyCreatedGamingGroupResult.NewlyCreatedGamingGroup = newGamingGroup;
            //commit changes since we'll need the GamingGroup.Id
            _dataContext.CommitAllChanges();

            var newlyCreatedPlayer = AssociateUserWithGamingGroup(currentUser, newGamingGroup);
            newlyCreatedGamingGroupResult.NewlyCreatedPlayer = newlyCreatedPlayer;

            new Task(() => _eventTracker.TrackGamingGroupCreation(registrationSource)).Start();

            return newlyCreatedGamingGroupResult;
        }

        private static void ValidateGamingGroupName(string gamingGroupName)
        {
            if (string.IsNullOrWhiteSpace(gamingGroupName))
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_GAMING_GROUP_NAME_CANNOT_BE_NULL_OR_BLANK);
            }
        }

        private Player AssociateUserWithGamingGroup(ApplicationUser currentUser, GamingGroup newGamingGroup)
        {
            AddUserGamingGroupRecord(currentUser, newGamingGroup);

            SetGamingGroupOnCurrentUser(currentUser, newGamingGroup);

            return AddUserToGamingGroupAsPlayer(currentUser);
        }

        private void AddUserGamingGroupRecord(ApplicationUser currentUser, GamingGroup newGamingGroup)
        {
            var userGamingGroup = new UserGamingGroup
            {
                ApplicationUserId = currentUser.Id,
                GamingGroupId = newGamingGroup.Id
            };

            _dataContext.Save(userGamingGroup, currentUser);
        }

        private void SetGamingGroupOnCurrentUser(ApplicationUser currentUser, GamingGroup newGamingGroup)
        {
            var user = _dataContext.FindById<ApplicationUser>(currentUser.Id);
            user.CurrentGamingGroupId = newGamingGroup.Id;
            _dataContext.Save(user, currentUser);

            currentUser.CurrentGamingGroupId = user.CurrentGamingGroupId;
        }

        private Player AddUserToGamingGroupAsPlayer(ApplicationUser currentUser)
        {
            var createPlayerRequest = new CreatePlayerRequest
            {
                Name = currentUser.UserName
            };
            return _playerSaver.CreatePlayer(createPlayerRequest, currentUser, true);
        }

        public GamingGroup UpdatePublicGamingGroupDetails(GamingGroupEditRequest request, ApplicationUser currentUser)
        {
            SwitchUsersCurrentGamingGroupIfNecessary(request);

            var gamingGroup = _dataContext.FindById<GamingGroup>(request.GamingGroupId);

            gamingGroup.PublicGamingGroupWebsite = request.Website;
            gamingGroup.PublicDescription = request.PublicDescription;
            gamingGroup.Name = request.GamingGroupName;
            gamingGroup.Active = request.Active;

            gamingGroup = _dataContext.Save(gamingGroup, currentUser);
            _dataContext.CommitAllChanges();

            _eventTracker.TrackGamingGroupUpdate(currentUser);

            return gamingGroup;
        }

        private void SwitchUsersCurrentGamingGroupIfNecessary(GamingGroupEditRequest request)
        {
            if (request.Active)
            {
                return;
            }
            var users = _dataContext.GetQueryable<ApplicationUser>()
                .Where(x => x.CurrentGamingGroupId == request.GamingGroupId)
                .ToList();
            foreach (var user in users)
            {
                _gamingGroupContextSwitcher.EnsureContextIsValid(user);
            }
        }
    }
}