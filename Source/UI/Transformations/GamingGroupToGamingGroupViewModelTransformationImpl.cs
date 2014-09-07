using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using System.Collections.Generic;
using UI.Models.GamingGroup;
using UI.Transformations.Player;

namespace UI.Transformations
{
    public class GamingGroupToGamingGroupViewModelTransformationImpl : GamingGroupToGamingGroupViewModelTransformation
    {
        private GamingGroupInvitationToInvitationViewModelTransformation invitationViewModelTransformer;
        private PlayerDetailsViewModelBuilder playerDetailsViewModelBuilder;

        public GamingGroupToGamingGroupViewModelTransformationImpl(
            GamingGroupInvitationToInvitationViewModelTransformation invitationViewModelTransformer,
            PlayerDetailsViewModelBuilder playerDetailsViewModelBuilder)
        {
            this.invitationViewModelTransformer = invitationViewModelTransformer;
            this.playerDetailsViewModelBuilder = playerDetailsViewModelBuilder;
        }

        public GamingGroupViewModel Build(GamingGroup gamingGroup, ApplicationUser currentUser = null)
        {
            List<InvitationViewModel> invitationViewModels = new List<InvitationViewModel>();
            foreach(GamingGroupInvitation invitation in gamingGroup.GamingGroupInvitations)
            {
                invitationViewModels.Add(invitationViewModelTransformer.Build(invitation));
            }
            
            GamingGroupViewModel viewModel = new GamingGroupViewModel()
            {
                Id = gamingGroup.Id,
                OwningUserId = gamingGroup.OwningUserId,
                Name = gamingGroup.Name,
                OwningUserName = gamingGroup.OwningUser.UserName,
                Invitations = invitationViewModels,
                Players = gamingGroup.Players,
                GameDefinitions = gamingGroup.GameDefinitions
            };

            return viewModel;
        }
    }
}