using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic.GameDefinitions;
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
    public class GamingGroupCreator : IGamingGroupCreator
    {
        internal const string EXCEPTION_MESSAGE_GAMING_GROUP_NAME_CANNOT_BE_NULL_OR_BLANK = "GamingGroup name cannot be null or blank";
        internal const string EXCEPTION_MESSAGE_PLAYER_NAMES_CANNOT_BE_NULL = "gamingGroupQuickStart.NewPlayerNames cannot be null.";
        internal const string EXCEPTION_MESSAGE_GAME_DEFINITION_NAMES_CANNOT_BE_NULL = "gamingGroupQuickStart.NewGameDefinitionNames cannot be null.";

        private IDataContext dataContext;
        private IPlayerSaver playerCreator;
        private IGameDefinitionSaver gameDefinitionCreator;
        private UserManager<ApplicationUser> userManager;
        private NemeStatsEventTracker eventTracker;

        public GamingGroupCreator(
            IDataContext dataContext, 
            UserManager<ApplicationUser> userManager, 
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

        public async Task<GamingGroup> CreateGamingGroupAsync(GamingGroupQuickStart gamingGroupQuickStart, ApplicationUser currentUser)
        {
            Validate(gamingGroupQuickStart);

            GamingGroup newGamingGroup = CreateNewGamingGroup(gamingGroupQuickStart.GamingGroupName, currentUser);

            await SetGamingGroupOnCurrentUser(currentUser, newGamingGroup);

            CreatePlayers(gamingGroupQuickStart, currentUser);

            CreateGameDefinitions(gamingGroupQuickStart, currentUser);

            new Task(() => eventTracker.TrackGamingGroupCreation()).Start();

            return newGamingGroup;
        }

        private static void Validate(GamingGroupQuickStart gamingGroupQuickStart)
        {
            ValidateGamingGroupQuickStartIsNotNull(gamingGroupQuickStart);
            ValidateGamingGroupName(gamingGroupQuickStart.GamingGroupName);
            ValidatePlayerNamesListIsNotNull(gamingGroupQuickStart);
            ValidateGameDefinitionNamesListIsNotNull(gamingGroupQuickStart);
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

        private static void ValidateGameDefinitionNamesListIsNotNull(GamingGroupQuickStart gamingGroupQuickStart)
        {
            if (gamingGroupQuickStart.NewGameDefinitionNames == null)
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_GAME_DEFINITION_NAMES_CANNOT_BE_NULL);
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
            Player newPlayer;
            foreach (string playerName in gamingGroupQuickStart.NewPlayerNames)
            {
                if(!string.IsNullOrWhiteSpace(playerName))
                {
                    newPlayer = new Player()
                    {
                        Name = playerName
                    };
                    playerCreator.Save(newPlayer, currentUser);
                }
            }
        }

        private void CreateGameDefinitions(GamingGroupQuickStart gamingGroupQuickStart, ApplicationUser currentUser)
        {
            GameDefinition gameDefinition;
            foreach (string gameDefinitionName in gamingGroupQuickStart.NewGameDefinitionNames)
            {
                if (!string.IsNullOrWhiteSpace(gameDefinitionName))
                {
                    gameDefinition = new GameDefinition() { Name = gameDefinitionName };
                    gameDefinitionCreator.Save(gameDefinition, currentUser);
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
