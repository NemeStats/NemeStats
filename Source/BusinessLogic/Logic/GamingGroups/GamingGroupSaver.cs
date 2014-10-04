using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.Players;
using BusinessLogic.Logic.Users;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversalAnalyticsHttpWrapper;

namespace BusinessLogic.Logic.GamingGroups
{
    public class GamingGroupSaver : IGamingGroupSaver
    {
        internal const string EXCEPTION_MESSAGE_GAMING_GROUP_NAME_CANNOT_BE_NULL_OR_BLANK = "GamingGroup name cannot be null or blank";
        internal const string EXCEPTION_MESSAGE_PLAYER_NAMES_CANNOT_BE_NULL = "gamingGroupQuickStart.NewPlayerNames cannot be null.";
        internal const string EXCEPTION_MESSAGE_GAME_DEFINITION_NAMES_CANNOT_BE_NULL = "gamingGroupQuickStart.NewGameDefinitionNames cannot be null.";

        private IDataContext dataContext;
        private IPlayerSaver playerCreator;
        private IGameDefinitionSaver gameDefinitionCreator;
        private ApplicationUserManager userManager;
        private NemeStatsEventTracker eventTracker;

        public GamingGroupSaver(
            IDataContext dataContext, 
            ApplicationUserManager userManager, 
            NemeStatsEventTracker eventTracker,
            IPlayerSaver playerCreator,
            IGameDefinitionSaver gameDefinitionCreator)
        {
            this.dataContext = dataContext;
            this.userManager = userManager;
            this.eventTracker = eventTracker;
            this.playerCreator = playerCreator;
            this.gameDefinitionCreator = gameDefinitionCreator;
        }

        public async virtual Task<GamingGroup> CreateNewGamingGroup(string gamingGroupName, ApplicationUser currentUser)
        {
            GamingGroup gamingGroup = new GamingGroup()
            {
                OwningUserId = currentUser.Id,
                Name = gamingGroupName
            };

            GamingGroup newGamingGroup = dataContext.Save<GamingGroup>(gamingGroup, currentUser);

            await SetGamingGroupOnCurrentUser(currentUser, newGamingGroup);

            new Task(() => eventTracker.TrackGamingGroupCreation()).Start();

            return newGamingGroup;
        }

        private async Task SetGamingGroupOnCurrentUser(ApplicationUser currentUser, GamingGroup newGamingGroup)
        {
            ApplicationUser user = await userManager.FindByIdAsync(currentUser.Id);
            user.CurrentGamingGroupId = newGamingGroup.Id;
            await userManager.UpdateAsync(user);

            currentUser.CurrentGamingGroupId = user.CurrentGamingGroupId;
        }

        public GamingGroup UpdateGamingGroupName(string gamingGroupName, ApplicationUser currentUser)
        {
            GamingGroup gamingGroup = dataContext.FindById<GamingGroup>(currentUser.CurrentGamingGroupId.Value);
            gamingGroup.Name = gamingGroupName;
            gamingGroup = dataContext.Save(gamingGroup, currentUser);
            dataContext.CommitAllChanges();
            return gamingGroup;
        }
    }
}
