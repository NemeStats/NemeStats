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

using System;
using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.Players;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.User;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.GamingGroups
{
    public class GamingGroupSaver : IGamingGroupSaver
    {
        internal const string EXCEPTION_MESSAGE_GAMING_GROUP_NAME_CANNOT_BE_NULL_OR_BLANK = "GamingGroup name cannot be null or blank";
        internal const string EXCEPTION_MESSAGE_PLAYER_NAMES_CANNOT_BE_NULL = "gamingGroupQuickStart.NewPlayerNames cannot be null.";
        internal const string EXCEPTION_MESSAGE_GAME_DEFINITION_NAMES_CANNOT_BE_NULL = "gamingGroupQuickStart.NewGameDefinitionNames cannot be null.";

        private readonly IDataContext dataContext;
        private readonly IPlayerSaver playerSaver;
        private readonly INemeStatsEventTracker eventTracker;

        public GamingGroupSaver(
            IDataContext dataContext, 
            INemeStatsEventTracker eventTracker, 
            IPlayerSaver playerSaver)
        {
            this.dataContext = dataContext;
            this.eventTracker = eventTracker;
            this.playerSaver = playerSaver;
        }

        public virtual NewlyCreatedGamingGroupResult CreateNewGamingGroup(string gamingGroupName, ApplicationUser currentUser)
        {
            ValidateGamingGroupName(gamingGroupName);

            GamingGroup gamingGroup = new GamingGroup()
            {
                OwningUserId = currentUser.Id,
                Name = gamingGroupName
            };

            NewlyCreatedGamingGroupResult newlyCreatedGamingGroupResult = new NewlyCreatedGamingGroupResult();
            GamingGroup newGamingGroup = dataContext.Save<GamingGroup>(gamingGroup, currentUser);
            newlyCreatedGamingGroupResult.NewlyCreatedGamingGroup = newGamingGroup;
            //commit changes since we'll need the GamingGroup.Id
            dataContext.CommitAllChanges();

            Player newlyCreatedPlayer = this.AssociateUserWithGamingGroup(currentUser, newGamingGroup);
            newlyCreatedGamingGroupResult.NewlyCreatedPlayer = newlyCreatedPlayer;

            new Task(() => eventTracker.TrackGamingGroupCreation()).Start();

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
            this.AddUserGamingGroupRecord(currentUser, newGamingGroup);

            this.SetGamingGroupOnCurrentUser(currentUser, newGamingGroup);

            return this.AddUserToGamingGroupAsPlayer(currentUser);
        }

        private void AddUserGamingGroupRecord(ApplicationUser currentUser, GamingGroup newGamingGroup)
        {
            UserGamingGroup userGamingGroup = new UserGamingGroup
            {
                ApplicationUserId = currentUser.Id,
                GamingGroupId = newGamingGroup.Id
            };

            this.dataContext.Save(userGamingGroup, currentUser);
        }

        private void SetGamingGroupOnCurrentUser(ApplicationUser currentUser, GamingGroup newGamingGroup)
        {
            ApplicationUser user = dataContext.FindById<ApplicationUser>(currentUser.Id);
            user.CurrentGamingGroupId = newGamingGroup.Id;
            dataContext.Save(user, currentUser);

            currentUser.CurrentGamingGroupId = user.CurrentGamingGroupId;
        }

        private Player AddUserToGamingGroupAsPlayer(ApplicationUser currentUser)
        {
            Player player = new Player
            {
                ApplicationUserId = currentUser.Id,
                Name = currentUser.UserName
            };
            return this.playerSaver.Save(player, currentUser);
        }

        public GamingGroup UpdateGamingGroupName(string gamingGroupName, ApplicationUser currentUser)
        {
            ValidateGamingGroupName(gamingGroupName);

            GamingGroup gamingGroup = dataContext.FindById<GamingGroup>(currentUser.CurrentGamingGroupId.Value);
            gamingGroup.Name = gamingGroupName;
            gamingGroup = dataContext.Save(gamingGroup, currentUser);
            dataContext.CommitAllChanges();
            return gamingGroup;
        }
    }
}
