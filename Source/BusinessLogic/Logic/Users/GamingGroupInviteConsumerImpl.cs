using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.GamingGroups;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.Users
{
    public class GamingGroupInviteConsumerImpl : GamingGroupInviteConsumer
    {
        private GamingGroupRepository gamingGroupRepository;
        private GamingGroupAccessGranter gamingGroupAccessGranter;
        private UserManager<ApplicationUser> userManager;

        public GamingGroupInviteConsumerImpl(
            GamingGroupRepository gamingGroupRepository, 
            UserManager<ApplicationUser> userManager,
            GamingGroupAccessGranter gamingGroupAccessGranter)
        {
            this.gamingGroupRepository = gamingGroupRepository;
            this.userManager = userManager;
            this.gamingGroupAccessGranter = gamingGroupAccessGranter;
        }

        public async Task<int?> AddUserToInvitedGroupAsync(ApplicationUser currentUser)
        {
            IList<GamingGroupInvitation> gamingGroupInvitations 
                = gamingGroupRepository.GetPendingGamingGroupInvitations(currentUser);
            
            if(gamingGroupInvitations.Count == 0)
            {
                return null;
            }
            
            ApplicationUser user = await userManager.FindByIdAsync(currentUser.Id);
            GamingGroupInvitation oldestInvite = gamingGroupInvitations.OrderBy(invite => invite.DateSent).First();
            user.CurrentGamingGroupId = oldestInvite.GamingGroupId;
            userManager.Update(user);

            gamingGroupAccessGranter.ConsumeInvitation(oldestInvite, currentUser);

            return user.CurrentGamingGroupId;
        }
    }
}
