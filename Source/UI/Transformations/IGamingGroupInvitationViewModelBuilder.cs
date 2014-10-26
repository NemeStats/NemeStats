using BusinessLogic.Models;
using System.Linq;
using UI.Models.GamingGroup;

namespace UI.Transformations
{
    public interface IGamingGroupInvitationViewModelBuilder
    {
        InvitationViewModel Build(GamingGroupInvitation invitation);
    }
}
