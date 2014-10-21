using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Logic.GamingGroups
{
    public class PendingGamingGroupInvitationRetriever : IPendingGamingGroupInvitationRetriever
    {
        private IDataContext dataContext;

        public PendingGamingGroupInvitationRetriever(IDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public virtual List<GamingGroupInvitation> GetPendingGamingGroupInvitations(ApplicationUser currentUser)
        {
            ApplicationUser user = dataContext.FindById<ApplicationUser>(currentUser.Id);

            return dataContext.GetQueryable<GamingGroupInvitation>()
                .Where(invitation => invitation.InviteeEmail == user.Email)
                .ToList();
        }
    }
}
