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

        public GamingGroup GetGamingGroupDetails(int gamingGroupId, ApplicationUser currentUser)
        {
            ValidateUserHasAccessToGamingGroup(gamingGroupId, currentUser);
            GamingGroup gamingGroup = dbContext.GamingGroups
                                        .Where(group => group.Id == gamingGroupId)
                                        .Include(group => group.OwningUser)
                                        .Include(group => group.GamingGroupInvitations)
                                        .FirstOrDefault();

            return gamingGroup;
        }

        //TODO this should probably be its own class. In fact, any place where we are doing a partial mock
        //like this then it should be busted out
        internal virtual void ValidateUserHasAccessToGamingGroup(int gamingGroupId, ApplicationUser currentUser)
        {
            if (gamingGroupId != currentUser.CurrentGamingGroupId)
            {
                //TODO Maybe do a custom exception so we don't have to do hacky testing here
                string message = string.Format(
                    EXCEPTION_MESSAGE_NO_ACCESS_TO_GAMING_GROUP,
                    currentUser.Id,
                    gamingGroupId);
                throw new UnauthorizedAccessException(message);
            }
        }

        public virtual IList<GamingGroupInvitation> GetPendingGamingGroupInvitations(ApplicationUser currentUser)
        {
            ApplicationUser user = dbContext.Users.Where(theUser => theUser.Id == currentUser.Id).FirstOrDefault();

            if(user == null)
            {
                string exceptionMessage = string.Format(EXCEPTION_MESSAGE_USER_DOES_NOT_EXIST, currentUser.Id);
                throw new KeyNotFoundException(exceptionMessage);
            }

            List<GamingGroupInvitation> invitations = dbContext.GamingGroupInvitations.Where(invitation => invitation.InviteeEmail == user.Email).ToList();

            return invitations;
        }

        public GamingGroup Save(GamingGroup gamingGroup, ApplicationUser currentUser)
        {
            if (gamingGroup.AlreadyInDatabase())
            {
                ValidateUserHasAccessToGamingGroup(gamingGroup.Id, currentUser);
                dbContext.Entry(gamingGroup).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                dbContext.GamingGroups.Add(gamingGroup);
            }

            dbContext.SaveChanges();

            return gamingGroup;
        }
    }
}
