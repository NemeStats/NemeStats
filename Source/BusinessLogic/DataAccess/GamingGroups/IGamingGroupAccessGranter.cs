using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DataAccess.GamingGroups
{
    public interface IGamingGroupAccessGranter
    {
        GamingGroupInvitation CreateInvitation(string email, ApplicationUser currentUser);
        GamingGroupInvitation ConsumeInvitation(GamingGroupInvitation gamingGroupInvitation, ApplicationUser currentUser);
    }
}
