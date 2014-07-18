using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.DataAccess.Repositories
{
    public class EntityFrameworkGamingGroupInvitationRepository : GamingGroupInvitationRepository
    {
        private NemeStatsDbContext dbContext;

        public EntityFrameworkGamingGroupInvitationRepository(NemeStatsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public GamingGroupInvitation Save(GamingGroupInvitation gamingGroupInvitation, UserContext userContext)
        {
            //TODO GamingGroupInvitation should implement a generic EntityWithTechnicalKey so we can take advantage of this shared logic elsewhere
            if (gamingGroupInvitation.Id != default(Guid))
            {
                dbContext.Entry(gamingGroupInvitation).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                dbContext.GamingGroupInvitations.Add(gamingGroupInvitation);
            }

            dbContext.SaveChanges();

            return gamingGroupInvitation;
        }
    }
}
