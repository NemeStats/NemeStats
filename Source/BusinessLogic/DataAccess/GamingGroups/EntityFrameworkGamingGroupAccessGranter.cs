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

        public GamingGroupInvitation CreateInvitation(string email, ApplicationUser currentUser)
        {
            GamingGroupInvitation invitation = new GamingGroupInvitation()
            {
                InviteeEmail = email,
                GamingGroupId = currentUser.CurrentGamingGroupId.Value,
                InvitingUserId = currentUser.Id,
                DateSent = DateTime.UtcNow.Date
            };
            gamingGroupInvitationRepository.Save(invitation, currentUser);

            return invitation;
        }


        public GamingGroupInvitation ConsumeInvitation(GamingGroupInvitation gamingGroupInvitation, ApplicationUser currentUser)
        {
            gamingGroupInvitation.DateRegistered = DateTime.UtcNow;
            gamingGroupInvitation.RegisteredUserId = currentUser.Id;
            gamingGroupInvitationRepository.Save(gamingGroupInvitation, currentUser);

            return gamingGroupInvitation;
        }
    }
}
