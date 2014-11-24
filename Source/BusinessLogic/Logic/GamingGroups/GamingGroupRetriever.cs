using BusinessLogic.DataAccess;
using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.User;
using System.Collections.Generic;
using System.Linq;

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

        public GamingGroupSummary GetGamingGroupDetails(int gamingGroupId, int maxNumberOfGamesToRetrieve)
        {
            GamingGroup gamingGroup = dataContext.FindById<GamingGroup>(gamingGroupId);
            GamingGroupSummary summary = new GamingGroupSummary
            {
                Id = gamingGroup.Id,
                DateCreated = gamingGroup.DateCreated,
                Name = gamingGroup.Name,
                OwningUserId = gamingGroup.OwningUserId
            };

            summary.PlayedGames = playedGameRetriever.GetRecentGames(maxNumberOfGamesToRetrieve, gamingGroupId);

            summary.Players = playerRetriever.GetAllPlayersWithNemesisInfo(gamingGroupId);

            summary.GameDefinitionSummaries = gameDefinitionRetriever.GetAllGameDefinitions(gamingGroupId);

            summary.OwningUser = dataContext.GetQueryable<ApplicationUser>().First(user => user.Id == gamingGroup.OwningUserId);

            return summary;
        }
    }
}
