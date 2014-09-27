using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using System.Collections.Generic;
using System.Linq;
using UI.Models.GamingGroup;
using UI.Models.PlayedGame;
using UI.Transformations.PlayerTransformations;
using UI.Views.Player;

namespace UI.Transformations
{
    public class GamingGroupViewModelBuilder : IGamingGroupViewModelBuilder
    {
        private IGamingGroupInvitationViewModelBuilder invitationViewModelTransformer;
        private IPlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilder;
        private IPlayerWithNemesisViewModelBuilder playerWithNemesisViewModelBuilder;

        public GamingGroupViewModelBuilder(
            IGamingGroupInvitationViewModelBuilder invitationViewModelTransformer,
            IPlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilder,
            IPlayerWithNemesisViewModelBuilder playerWithNemesisViewModelBuilder)
        {
            this.invitationViewModelTransformer = invitationViewModelTransformer;
            this.playedGameDetailsViewModelBuilder = playedGameDetailsViewModelBuilder;
            this.playerWithNemesisViewModelBuilder = playerWithNemesisViewModelBuilder;
        }

        public GamingGroupViewModel Build(GamingGroup gamingGroup, ApplicationUser currentUser = null)
        {
            List<InvitationViewModel> invitationViewModels
                = (from GamingGroupInvitation invitation in gamingGroup.GamingGroupInvitations
                   select invitationViewModelTransformer.Build(invitation)).ToList();

            List<PlayedGameDetailsViewModel> details = BuildPlayedGameDetailsViewModels(gamingGroup, currentUser);

            List<PlayerWithNemesisViewModel> playerWithNemesisList 
                = (from Player player in gamingGroup.Players
                  select playerWithNemesisViewModelBuilder.Build(player)).ToList();
            
            GamingGroupViewModel viewModel = new GamingGroupViewModel()
            {
                Id = gamingGroup.Id,
                OwningUserId = gamingGroup.OwningUserId,
                Name = gamingGroup.Name,
                OwningUserName = gamingGroup.OwningUser.UserName,
                Invitations = invitationViewModels,
                Players = playerWithNemesisList,
                GameDefinitions = gamingGroup.GameDefinitions,
                RecentGames = details
            };

            return viewModel;
        }

        private List<PlayedGameDetailsViewModel> BuildPlayedGameDetailsViewModels(GamingGroup gamingGroup, ApplicationUser currentUser)
        {
            int totalGames = gamingGroup.PlayedGames.Count;
            List<PlayedGameDetailsViewModel> details = new List<PlayedGameDetailsViewModel>(totalGames);
            for (int i = 0; i < totalGames; i++)
            {
                details.Add(playedGameDetailsViewModelBuilder.Build(gamingGroup.PlayedGames[i], currentUser));
            }
            return details;
        }
    }
}