using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.GamingGroups;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;

namespace BusinessLogic.Logic.Users
{
    public class GamingGroupInviteConsumer : IGamingGroupInviteConsumer
    {
        internal const string EXCEPTION_MESSAGE_KEY_NOT_FOUND = "GamingGroupInvitation with Id '{0}' could not be found.";
        private readonly IPendingGamingGroupInvitationRetriever pendingGamingGroupRetriever;
        private readonly IGamingGroupAccessGranter gamingGroupAccessGranter;
        private readonly ApplicationUserManager userManager;
        private readonly IDataContext dataContext;

        public GamingGroupInviteConsumer(
            IPendingGamingGroupInvitationRetriever pendingGamingGroupRetriever,
            ApplicationUserManager userManager,
            IGamingGroupAccessGranter gamingGroupAccessGranter,
            IDataContext dataContext)
        {
            this.pendingGamingGroupRetriever = pendingGamingGroupRetriever;
            this.userManager = userManager;
            this.gamingGroupAccessGranter = gamingGroupAccessGranter;
            this.dataContext = dataContext;
        }

        public async Task<int?> ConsumeGamingGroupInvitation(ApplicationUser currentUser)
        {
            IList<GamingGroupInvitation> gamingGroupInvitations
                = pendingGamingGroupRetriever.GetPendingGamingGroupInvitations(currentUser);

            if (gamingGroupInvitations.Count == 0)
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

        public bool ConsumeInvitation(string gamingGroupInvitationId)
        {
            var invitation = this.ValidateGamingGroupInvitation(gamingGroupInvitationId);

            if (invitation.RegisteredUserId == null)
            {
                return false;
            }

            var existingUser = this.ValidateExistingUser(invitation);

            this.AddNewGamingGroupAssociation(invitation);
            this.SwitchCurrentGamingGroup(existingUser, invitation);

            return true;
        }

        private GamingGroupInvitation ValidateGamingGroupInvitation(string gamingGroupInvitationId)
        {
            var invitation = this.dataContext.FindById<GamingGroupInvitation>(new Guid(gamingGroupInvitationId));

            if (invitation == null)
            {
                throw new EntityDoesNotExistException(gamingGroupInvitationId);
            }
            return invitation;
        }

        private ApplicationUser ValidateExistingUser(GamingGroupInvitation invitation)
        {
            var existingUser = this.dataContext.FindById<ApplicationUser>(invitation.RegisteredUserId);

            if (existingUser == null)
            {
                throw new EntityDoesNotExistException(invitation.RegisteredUserId);
            }
            return existingUser;
        }

        private void AddNewGamingGroupAssociation(GamingGroupInvitation invitation)
        {
            var newGamingGroupAssociation = new UserGamingGroup
            {
                ApplicationUserId = invitation.RegisteredUserId,
                GamingGroupId = invitation.GamingGroupId
            };

            //ApplicationUser is not used when saving new entities -- just has to be not-null
            this.dataContext.Save(newGamingGroupAssociation, new ApplicationUser());
        }

        private void SwitchCurrentGamingGroup(ApplicationUser existingUser, GamingGroupInvitation invitation)
        {
            existingUser.CurrentGamingGroupId = invitation.GamingGroupId;
            this.dataContext.Save(existingUser, new ApplicationUser());
        }
    }
}