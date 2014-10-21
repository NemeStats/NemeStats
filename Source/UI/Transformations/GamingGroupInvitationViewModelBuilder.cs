using BusinessLogic.Models;
using System.Linq;
using UI.Models.GamingGroup;

namespace UI.Transformations
{
    public class GamingGroupInvitationViewModelBuilder : IGamingGroupInvitationViewModelBuilder
    {
        public InvitationViewModel Build(GamingGroupInvitation invitation)
        {
            InvitationViewModel viewModel = new InvitationViewModel()
            {
                InviteeEmail = invitation.InviteeEmail,
                DateRegistered = invitation.DateRegistered
            };

            if(invitation.RegisteredUser == null)
            {
                viewModel.RegisteredUserName = string.Empty;
            }else
            {
                viewModel.RegisteredUserName = invitation.RegisteredUser.UserName;
            }

            return viewModel;
        }
    }
}