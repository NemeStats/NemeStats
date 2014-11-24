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
        internal const string EXCEPTION_MESSAGE_USER_MUST_ALREADY_EXIST = "The passed in user must already be registered and must have an Id.";
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

        public AddUserToGamingGroupResult AddExistingUserToGamingGroup(string gamingGroupInvitationId)
        {
            var invitation = this.ValidateGamingGroupInvitation(gamingGroupInvitationId);
            AddUserToGamingGroupResult result = new AddUserToGamingGroupResult
            {
                EmailAddress = invitation.InviteeEmail
            };

            if (invitation.RegisteredUserId == null)
            {
                result.UserAddedToExistingGamingGroup = false;
            }
            else
            {
                var existingUser = this.ValidateExistingUser(invitation);

                this.AddNewGamingGroupAssociation(invitation);
                this.SwitchCurrentGamingGroup(existingUser, invitation);
                result.UserAddedToExistingGamingGroup = true;
            }

            return result;
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

        public void AddNewUserToGamingGroup(string applicationUserId, Guid gamingGroupInvitationId)
        {
            ApplicationUser userFromDatabase = dataContext.FindById<ApplicationUser>(applicationUserId);

            ValidateApplicationUser(userFromDatabase, applicationUserId);

            GamingGroupInvitation invitation = dataContext.FindById<GamingGroupInvitation>(gamingGroupInvitationId);

            ValidateInvitation(gamingGroupInvitationId, invitation);

            this.AssociateUserWithNewGamingGroup(invitation.GamingGroupId, userFromDatabase);

            Player player = dataContext.FindById<Player>(invitation.PlayerId);

            ValidatePlayer(player, invitation);

            player.ApplicationUserId = applicationUserId;
            dataContext.Save(player, userFromDatabase);

            dataContext.CommitAllChanges();
        }

        private static void ValidateApplicationUser(ApplicationUser userFromDatabase, string userId)
        {
            if (userFromDatabase == null)
            {
                throw new EntityDoesNotExistException(userId);
            }
        }

        private static void ValidateInvitation(Guid guid, GamingGroupInvitation invitation)
        {
            if (invitation == null)
            {
                throw new EntityDoesNotExistException(guid);
            }
        }

        private void AssociateUserWithNewGamingGroup(int gamingGroupId, ApplicationUser userFromDatabase)
        {
            UserGamingGroup userGamingGroup = new UserGamingGroup
            {
                ApplicationUserId = userFromDatabase.Id,
                GamingGroupId = gamingGroupId
            };

            this.dataContext.Save(userGamingGroup, userFromDatabase);

            userFromDatabase.CurrentGamingGroupId = gamingGroupId;
            this.dataContext.Save(userFromDatabase, userFromDatabase);
        }

        private static void ValidatePlayer(Player player, GamingGroupInvitation invitation)
        {
            if (player == null)
            {
                throw new EntityDoesNotExistException(invitation.PlayerId);
            }
        }
    }
}