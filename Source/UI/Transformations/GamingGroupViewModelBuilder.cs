using BusinessLogic.Models;
using BusinessLogic.Models.GamingGroups;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using System.Collections.Generic;
using System.Linq;
using UI.Models.GamingGroup;
using UI.Models.PlayedGame;
using UI.Models.Players;
using UI.Transformations.PlayerTransformations;

namespace UI.Transformations
{
    public class GamingGroupViewModelBuilder : IGamingGroupViewModelBuilder
    {
        private readonly IPlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilder;
        private readonly IPlayerWithNemesisViewModelBuilder playerWithNemesisViewModelBuilder;
        private readonly IGameDefinitionSummaryViewModelBuilder gameDefinitionSummaryViewModelBuilder;

        public GamingGroupViewModelBuilder(
            IPlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilder,
            IPlayerWithNemesisViewModelBuilder playerWithNemesisViewModelBuilder,
            IGameDefinitionSummaryViewModelBuilder gameDefinitionSummaryViewModelBuilder)
        {
            this.playedGameDetailsViewModelBuilder = playedGameDetailsViewModelBuilder;
            this.playerWithNemesisViewModelBuilder = playerWithNemesisViewModelBuilder;
            this.gameDefinitionSummaryViewModelBuilder = gameDefinitionSummaryViewModelBuilder;
        }

        public GamingGroupViewModel Build(GamingGroupSummary gamingGroupSummary, ApplicationUser currentUser = null)
        {
            List<PlayedGameDetailsViewModel> details = BuildPlayedGameDetailsViewModels(gamingGroupSummary, currentUser);

            List<PlayerWithNemesisViewModel> playerWithNemesisList 
                = (from PlayerWithNemesis playerWithNemesis in gamingGroupSummary.Players
                   select playerWithNemesisViewModelBuilder.Build(playerWithNemesis, currentUser)).ToList();
            
            GamingGroupViewModel viewModel = new GamingGroupViewModel()
            {
                Id = gamingGroupSummary.Id,
                OwningUserId = gamingGroupSummary.OwningUserId,
                Name = gamingGroupSummary.Name,
                OwningUserName = gamingGroupSummary.OwningUser.UserName,
                Players = playerWithNemesisList,
                GameDefinitionSummaries = gamingGroupSummary.GameDefinitionSummaries
                    .Select(game => gameDefinitionSummaryViewModelBuilder.Build(game, currentUser)).ToList(),
                RecentGames = details
            };

            return viewModel;
        }

        private List<PlayedGameDetailsViewModel> BuildPlayedGameDetailsViewModels(GamingGroupSummary gamingGroupSummary, ApplicationUser currentUser)
        {
            int totalGames = gamingGroupSummary.PlayedGames.Count;
            List<PlayedGameDetailsViewModel> details = new List<PlayedGameDetailsViewModel>(totalGames);
            for (int i = 0; i < totalGames; i++)
            {
                details.Add(playedGameDetailsViewModelBuilder.Build(gamingGroupSummary.PlayedGames[i], currentUser));
            }
            return details;
        }
    }
}