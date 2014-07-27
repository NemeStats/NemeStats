using BusinessLogic.Models;
using System.Collections.Generic;
using UI.Models.GamingGroup;

namespace UI.Transformations
{
    public class GamingGroupToGamingGroupViewModelTransformationImpl : GamingGroupToGamingGroupViewModelTransformation
    {
        private GamingGroupInvitationToInvitationViewModelTransformation invitationViewModelTransformer;

        public GamingGroupToGamingGroupViewModelTransformationImpl(
            GamingGroupInvitationToInvitationViewModelTransformation invitationViewModelTransformer)
        {
            this.invitationViewModelTransformer = invitationViewModelTransformer;
        }

        public GamingGroupViewModel Build(GamingGroup gamingGroup)
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
                Invitations = invitationViewModels
            };

            return viewModel;
        }
    }
}