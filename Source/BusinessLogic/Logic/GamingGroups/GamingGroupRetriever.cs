using BusinessLogic.DataAccess;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.GamingGroups
{
    public class GamingGroupRetriever : IGamingGroupRetriever
    {
        private readonly IDataContext dataContext;
        private readonly IPlayerRetriever playerRetriever;
        private readonly IGameDefinitionRetriever gameDefinitionRetriever;
        private readonly IPlayedGameRetriever playedGameRetriever;

        public GamingGroupRetriever(
            IDataContext dataContext, 
            IPlayerRetriever playerRetriever, 
            IGameDefinitionRetriever gameDefinitionRetriever,
            IPlayedGameRetriever playedGameRetriever)
        {
            this.dataContext = dataContext;
            this.playerRetriever = playerRetriever;
            this.gameDefinitionRetriever = gameDefinitionRetriever;
            this.playedGameRetriever = playedGameRetriever;
        }

        public GamingGroup GetGamingGroupDetails(int gamingGroupId, int maxNumberOfGamesToRetrieve)
        {
            GamingGroup gamingGroup = dataContext.FindById<GamingGroup>(gamingGroupId);

            gamingGroup.PlayedGames = playedGameRetriever.GetRecentGames(maxNumberOfGamesToRetrieve, gamingGroupId);

            gamingGroup.Players = playerRetriever.GetAllPlayersWithNemesisInfo(gamingGroupId);

            gamingGroup.GameDefinitions = gameDefinitionRetriever.GetAllGameDefinitions(gamingGroupId);

            gamingGroup.OwningUser = dataContext.GetQueryable<ApplicationUser>()
                .Where(user => user.Id == gamingGroup.OwningUserId)
                .First();

            gamingGroup.GamingGroupInvitations = dataContext.GetQueryable<GamingGroupInvitation>()
                .Where(invitation => invitation.GamingGroupId == gamingGroup.Id)
                .ToList();

            AddRegisteredUserInfo(gamingGroup);

            return gamingGroup;
        }

        private void AddRegisteredUserInfo(GamingGroup gamingGroup)
        {
            List<string> registeredUserIds = (from gamingGroupInvitation in gamingGroup.GamingGroupInvitations
                                              select gamingGroupInvitation.RegisteredUserId).ToList();

            List<ApplicationUser> registeredUsers = dataContext.GetQueryable<ApplicationUser>()
                .Where(user => registeredUserIds.Contains(user.Id))
                .ToList();

            foreach (GamingGroupInvitation gamingGroupInvitation in gamingGroup.GamingGroupInvitations)
            {
                gamingGroupInvitation.RegisteredUser = registeredUsers
                    .Where(user => user.Id == gamingGroupInvitation.RegisteredUserId)
                    .FirstOrDefault();
            }
        }
    }
}
