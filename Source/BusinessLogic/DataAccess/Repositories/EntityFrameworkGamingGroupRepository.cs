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

        public EntityFrameworkGamingGroupRepository(NemeStatsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public GamingGroup GetGamingGroupDetails(int gamingGroupId, UserContext userContext)
        {
            GamingGroup gamingGroup = dbContext.GamingGroups
                                        .Where(group => group.Id == gamingGroupId)
                                        .Include(group => group.OwningUser)
                                        .FirstOrDefault();

            return gamingGroup;
        }
    }
}
