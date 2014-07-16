using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UI.Models.GamingGroup;
using System.Collections.Generic;

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