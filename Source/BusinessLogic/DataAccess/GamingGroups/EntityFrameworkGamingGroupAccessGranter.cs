using BusinessLogic.DataAccess.Repositories;
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
        protected GamingGroupInvitationRepository gamingGroupInvitationRepository;

        public EntityFrameworkGamingGroupAccessGranter(NemeStatsDbContext dbContext, GamingGroupInvitationRepository gamingGroupInvitationRepository)
        {
            this.dbContext = dbContext;
            this.gamingGroupInvitationRepository = gamingGroupInvitationRepository;
        }

        public GamingGroupInvitation CreateInvitation(string email, UserContext userContext)
        {
            GamingGroupInvitation invitation = new GamingGroupInvitation()
            {
                InviteeEmail = email,
                GamingGroupId = userContext.GamingGroupId.Value,
                InvitingUserId = userContext.ApplicationUserId,
                DateSent = DateTime.UtcNow.Date
            };
            gamingGroupInvitationRepository.Save(invitation, userContext);

            return invitation;
        }


        public GamingGroupInvitation ConsumeInvitation(GamingGroupInvitation gamingGroupInvitation, UserContext userContext)
        {
            gamingGroupInvitation.DateRegistered = DateTime.UtcNow;
            gamingGroupInvitation.RegisteredUserId = userContext.ApplicationUserId;
            gamingGroupInvitationRepository.Save(gamingGroupInvitation, userContext);

            return gamingGroupInvitation;
        }
    }
}
