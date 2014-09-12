using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.GamingGroups;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.GamingGroups;
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
    public class GamingGroupInviteConsumer : IGamingGroupInviteConsumer
    {
        private IPendingGamingGroupInvitationRetriever pendingGamingGroupRetriever;
        private IGamingGroupAccessGranter gamingGroupAccessGranter;
        private UserManager<ApplicationUser> userManager;

        public GamingGroupInviteConsumer(
            IPendingGamingGroupInvitationRetriever pendingGamingGroupRetriever, 
            UserManager<ApplicationUser> userManager,
            IGamingGroupAccessGranter gamingGroupAccessGranter)
        {
            this.pendingGamingGroupRetriever = pendingGamingGroupRetriever;
            this.userManager = userManager;
            this.gamingGroupAccessGranter = gamingGroupAccessGranter;
        }

        public async Task<int?> AddUserToInvitedGroupAsync(ApplicationUser currentUser)
        {
            IList<GamingGroupInvitation> gamingGroupInvitations 
                = pendingGamingGroupRetriever.GetPendingGamingGroupInvitations(currentUser);
            
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
