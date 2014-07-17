using BusinessLogic.DataAccess;
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
        private UserManager<ApplicationUser> userManager;

        public GamingGroupInviteConsumerImpl(GamingGroupRepository gamingGroupRepository, UserManager<ApplicationUser> userManager)
        {
            this.gamingGroupRepository = gamingGroupRepository;
            this.userManager = userManager;
        }

        public async Task<int?> AddUserToInvitedGroupAsync(UserContext userContext)
        {
            IList<GamingGroupInvitation> gamingGroupInvitations 
                = gamingGroupRepository.GetPendingGamingGroupInvitations(userContext);
            
            if(gamingGroupInvitations.Count == 0)
            {
                return null;
            }
            
            ApplicationUser user = await userManager.FindByIdAsync(userContext.ApplicationUserId);
            user.CurrentGamingGroupId = gamingGroupInvitations.First().GamingGroupId;
            userManager.Update(user);

            return user.CurrentGamingGroupId;
        }
    }
}
