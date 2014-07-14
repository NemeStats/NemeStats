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

        public EntityFrameworkGamingGroupRepository(NemeStatsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public GamingGroup GetGamingGroupDetails(int gamingGroupId, UserContext userContext)
        {
            if(gamingGroupId != userContext.GamingGroupId)
            {
                string message = string.Format(
                    EXCEPTION_MESSAGE_NO_ACCESS_TO_GAMING_GROUP, 
                    userContext.ApplicationUserId, 
                    gamingGroupId);
                throw new UnauthorizedAccessException(message);
            }
            GamingGroup gamingGroup = dbContext.GamingGroups
                                        .Where(group => group.Id == gamingGroupId)
                                        .Include(group => group.OwningUser)
                                        .FirstOrDefault();

            return gamingGroup;
        }
    }
}
