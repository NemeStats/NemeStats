using System;
using System.Collections.Generic;
using System.Configuration.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;

namespace BusinessLogic.Logic.Players
{
    public class PlayerInviter : IPlayerInviter
    {
        internal const string APP_SETTING_URL_ROOT = "urlRoot";
        internal const string EMAIL_MESSAGE_INVITE_PLAYER = "Hi There! You've been invited by {0} to join the \"{1}\" gaming group on {2}. {5}"
                                  + "{0} says: {3} {5} "
                                  + "To join this Gaming Group click on this link: {2}/Account/ConsumeInvitation/{4}";

        private readonly IDataContext dataContext;
        private readonly IIdentityMessageService emailService;
        private readonly IConfigurationManager configurationManager;

        public PlayerInviter(IDataContext dataContext, IIdentityMessageService emailService, IConfigurationManager configurationManager)
        {
            this.dataContext = dataContext;
            this.emailService = emailService;
            this.configurationManager = configurationManager;
        }

        public void InvitePlayer(PlayerInvitation playerInvitation, ApplicationUser currentUser)
        {
            GamingGroup gamingGroup = dataContext.FindById<GamingGroup>(currentUser.CurrentGamingGroupId.Value);

            string existingUserId = (from ApplicationUser user in dataContext.GetQueryable<ApplicationUser>()
                                     where user.Email == playerInvitation.InvitedPlayerEmail
                                     select user.Id).FirstOrDefault();       
            
            GamingGroupInvitation gamingGroupInvitation = new GamingGroupInvitation
            {
                DateSent = DateTime.UtcNow,
                GamingGroupId = currentUser.CurrentGamingGroupId.Value,
                InviteeEmail = playerInvitation.InvitedPlayerEmail,
                InvitingUserId = currentUser.Id,
                PlayerId = playerInvitation.InvitedPlayerId,
                RegisteredUserId = existingUserId
            };

            GamingGroupInvitation savedGamingGroupInvitation = dataContext.Save<GamingGroupInvitation>(gamingGroupInvitation, currentUser);
            //commit so we can get the Id back
            dataContext.CommitAllChanges();

            string urlRoot = configurationManager.AppSettings[APP_SETTING_URL_ROOT];

            string messageBody = string.Format(PlayerInviter.EMAIL_MESSAGE_INVITE_PLAYER,
                                                currentUser.UserName,
                                                gamingGroup.Name,
                                                urlRoot,
                                                playerInvitation.CustomEmailMessage,
                                                savedGamingGroupInvitation.Id,
                                                "<br/><br/>");
            var message = new IdentityMessage
            {
                Body = messageBody,
                Destination = playerInvitation.InvitedPlayerEmail,
                Subject = playerInvitation.EmailSubject
            };
            
            emailService.SendAsync(message);
        }
    }
}
