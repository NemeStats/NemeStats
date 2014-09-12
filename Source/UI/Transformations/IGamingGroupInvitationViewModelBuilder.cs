using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Models.GamingGroup;

namespace UI.Transformations
{
    public interface IGamingGroupInvitationViewModelBuilder
    {
        InvitationViewModel Build(GamingGroupInvitation invitation);
    }
}
