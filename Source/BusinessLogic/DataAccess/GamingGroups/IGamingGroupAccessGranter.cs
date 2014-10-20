using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System.Linq;

namespace BusinessLogic.DataAccess.GamingGroups
{
    public interface IGamingGroupAccessGranter
    {
        GamingGroupInvitation CreateInvitation(string email, ApplicationUser currentUser);
        GamingGroupInvitation ConsumeInvitation(GamingGroupInvitation gamingGroupInvitation, ApplicationUser currentUser);
    }
}
