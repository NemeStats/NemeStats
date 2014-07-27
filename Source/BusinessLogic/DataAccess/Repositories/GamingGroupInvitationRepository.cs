using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DataAccess.Repositories
{
    public interface GamingGroupInvitationRepository
    {
        GamingGroupInvitation Save(GamingGroupInvitation gamingGroupInvitation, ApplicationUser currentUser);
    }
}
