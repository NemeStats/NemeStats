using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using System.Collections.Generic;
using UI.Models.GamingGroup;
using UI.Models.PlayedGame;
using UI.Transformations.Player;

namespace UI.Transformations
{
    public class GamingGroupViewModelBuilder : IGamingGroupViewModelBuilder
    {
        private IGamingGroupInvitationViewModelBuilder invitationViewModelTransformer;
        private IPlayerDetailsViewModelBuilder playerDetailsViewModelBuilder;
        private IPlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilder;

        public GamingGroupViewModelBuilder(
            IGamingGroupInvitationViewModelBuilder invitationViewModelTransformer,
            IPlayerDetailsViewModelBuilder playerDetailsViewModelBuilder,
            IPlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilder)
        {
            this.invitationViewModelTransformer = invitationViewModelTransformer;
            this.playerDetailsViewModelBuilder = playerDetailsViewModelBuilder;
            this.playedGameDetailsViewModelBuilder = playedGameDetailsViewModelBuilder;
        }

        public GamingGroupViewModel Build(GamingGroup gamingGroup, ApplicationUser currentUser = null)
        {
            List<InvitationViewModel> invitationViewModels = new List<InvitationViewModel>();
            foreach(GamingGroupInvitation invitation in gamingGroup.GamingGroupInvitations)
            {
                invitationViewModels.Add(invitationViewModelTransformer.Build(invitation));
            }

            List<PlayedGameDetailsViewModel> details = BuildPlayedGameDetailsViewModels(gamingGroup, currentUser);
            
            GamingGroupViewModel viewModel = new GamingGroupViewModel()
            {
                Id = gamingGroup.Id,
                OwningUserId = gamingGroup.OwningUserId,
                Name = gamingGroup.Name,
                OwningUserName = gamingGroup.OwningUser.UserName,
                Invitations = invitationViewModels,
                Players = gamingGroup.Players,
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