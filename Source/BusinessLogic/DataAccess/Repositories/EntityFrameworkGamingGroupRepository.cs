using BusinessLogic.Exceptions;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DataAccess.Repositories
{
    public class EntityFrameworkGamingGroupRepository : GamingGroupRepository
    {
        private NemeStatsDbContext dbContext;
        internal const string EXCEPTION_MESSAGE_NO_ACCESS_TO_GAMING_GROUP 
            = "User with Id '{0} does not have access to Gaming Group with Id '{1}'.";
        internal const string EXCEPTION_MESSAGE_USER_DOES_NOT_EXIST = "User with Id '{0}' does not exist.";

        public EntityFrameworkGamingGroupRepository(NemeStatsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public virtual IList<GamingGroupInvitation> GetPendingGamingGroupInvitations(ApplicationUser currentUser)
        {
            ApplicationUser user = dbContext.Users.Where(theUser => theUser.Id == currentUser.Id).FirstOrDefault();

            if (user == null)
            {
                string exceptionMessage = string.Format(EXCEPTION_MESSAGE_USER_DOES_NOT_EXIST, currentUser.Id);
                throw new KeyNotFoundException(exceptionMessage);
            }

            List<GamingGroupInvitation> invitations = dbContext.GamingGroupInvitations.Where(invitation => invitation.InviteeEmail == user.Email).ToList();

            return invitations;
        }
    }
}
