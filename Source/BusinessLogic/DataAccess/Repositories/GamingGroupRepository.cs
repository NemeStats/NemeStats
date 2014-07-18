using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DataAccess.Repositories
{
    public interface GamingGroupRepository
    {
        GamingGroup GetGamingGroupDetails(int gamingGroupId, UserContext userContext);
        IList<GamingGroupInvitation> GetPendingGamingGroupInvitations(UserContext userContext);
        GamingGroup Save(GamingGroup gamingGroup, UserContext userContext);
    }
}
