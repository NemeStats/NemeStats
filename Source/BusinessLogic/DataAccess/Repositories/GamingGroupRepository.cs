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
        GamingGroup GetGamingGroupDetails(int gamingGroupId, ApplicationUser currentUser);
        IList<GamingGroupInvitation> GetPendingGamingGroupInvitations(ApplicationUser currentUser);
        GamingGroup Save(GamingGroup gamingGroup, ApplicationUser currentUser);
    }
}
