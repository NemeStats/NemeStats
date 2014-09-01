using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.GamingGroups;
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
    public class GamingGroupCreatorImpl : GamingGroupCreator
    {
        internal const string EXCEPTION_MESSAGE_GAMING_GROUP_NAME_CANNOT_BE_NULL_OR_BLANK = "GamingGroup name cannot be null or blank";
        internal const string EXCEPTION_MESSAGE_PLAYER_NAMES_CANNOT_BE_NULL = "gamingGroupQuickStart.NewPlayerNames cannot be null.";

        private DataContext dataContext;
        private PlayerCreator playerCreator;
        private UserManager<ApplicationUser> userManager;
        private NemeStatsEventTracker eventTracker;

        public GamingGroupCreatorImpl(
            DataContext dataContext, 
            UserManager<ApplicationUser> userManager, 
            NemeStatsEventTracker eventTracker,
            PlayerCreator playerCreator)
        {
            this.dataContext = dataContext;
            this.userManager = userManager;
            this.eventTracker = eventTracker;
            this.playerCreator = playerCreator;
        }

        public async Task<GamingGroup> CreateGamingGroupAsync(GamingGroupQuickStart gamingGroupQuickStart, ApplicationUser currentUser)
        {
            Validate(gamingGroupQuickStart);

            GamingGroup newGamingGroup = CreateNewGamingGroup(gamingGroupQuickStart.GamingGroupName, currentUser);

            await SetGamingGroupOnCurrentUser(currentUser, newGamingGroup);

            CreatePlayers(gamingGroupQuickStart, currentUser);

            new Task(() => eventTracker.TrackGamingGroupCreation()).Start();

            return newGamingGroup;
        }

        private static void Validate(GamingGroupQuickStart gamingGroupQuickStart)
        {
            ValidateGamingGroupQuickStartIsNotNull(gamingGroupQuickStart);
            ValidateGamingGroupName(gamingGroupQuickStart.GamingGroupName);
            ValidatePlayerNamesListIsNotNull(gamingGroupQuickStart);
        }

        private static void ValidateGamingGroupQuickStartIsNotNull(GamingGroupQuickStart gamingGroupQuickStart)
        {
            if (gamingGroupQuickStart == null)
            {
                throw new ArgumentNullException("gamingGroupQuickStart");
            }
        }

        private static void ValidateGamingGroupName(string gamingGroupName)
        {
            if (string.IsNullOrWhiteSpace(gamingGroupName))
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_GAMING_GROUP_NAME_CANNOT_BE_NULL_OR_BLANK);
            }
        }

        private static void ValidatePlayerNamesListIsNotNull(GamingGroupQuickStart gamingGroupQuickStart)
        {
            if (gamingGroupQuickStart.NewPlayerNames == null)
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_PLAYER_NAMES_CANNOT_BE_NULL);
            }
        }

        private GamingGroup CreateNewGamingGroup(string gamingGroupName, ApplicationUser currentUser)
        {
            GamingGroup gamingGroup = new GamingGroup()
            {
                OwningUserId = currentUser.Id,
                Name = gamingGroupName
            };

            return dataContext.Save<GamingGroup>(gamingGroup, currentUser);
        }

        private void CreatePlayers(GamingGroupQuickStart gamingGroupQuickStart, ApplicationUser currentUser)
        {
            foreach (string playerName in gamingGroupQuickStart.NewPlayerNames)
            {
                if(!string.IsNullOrWhiteSpace(playerName))
                {
                    playerCreator.CreatePlayer(playerName, currentUser);
                }
            }
        }

        private async Task SetGamingGroupOnCurrentUser(ApplicationUser currentUser, GamingGroup newGamingGroup)
        {
            ApplicationUser user = await userManager.FindByIdAsync(currentUser.Id);
            user.CurrentGamingGroupId = newGamingGroup.Id;
            await userManager.UpdateAsync(user);

            currentUser.CurrentGamingGroupId = user.CurrentGamingGroupId;
        }
    }
}
