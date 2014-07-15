using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DataAccess.GamingGroups
{
    public class EntityFrameworkGamingGroupAccessGranter : GamingGroupAccessGranter
    {
        protected NemeStatsDbContext dbContext;

        public EntityFrameworkGamingGroupAccessGranter(NemeStatsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void GrantAccess(string email, UserContext userContext)
        {
            GamingGroupInvitation invitation = new GamingGroupInvitation()
            {
                InviteeEmail = email,
                GamingGroupId = userContext.GamingGroupId.Value,
                InvitingUserId = userContext.ApplicationUserId,
                DateSent = DateTime.UtcNow.Date
            };
            dbContext.GamingGroupInvitations.Add(invitation);
            dbContext.SaveChanges();
        }
    }
}
