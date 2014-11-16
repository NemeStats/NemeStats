using System;
using System.Collections.Generic;
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
        internal const string EMAIL_MESSAGE_INVITE_PLAYER = "{0} invited you to join the Gaming Group \"{1}\" on {2}/GamingGroup/Details/{3}. \r\n\r\n "
                                  + "{4}\r\n\r\n "
                                  + "To join this Gaming Group click on this link: {2}/Account/ConsumeInvitation/{5}";

        private readonly IDataContext dataContext;
        private readonly IIdentityMessageService emailService;

        public PlayerInviter(IDataContext dataContext, IIdentityMessageService emailService)
        {
            this.dataContext = dataContext;
            this.emailService = emailService;
        }
        public void InvitePlayer(PlayerInvitation playerInvitation, ApplicationUser currentUser)
        {
            GamingGroup gamingGroup = dataContext.FindById<GamingGroup>(currentUser.CurrentGamingGroupId.Value);

            GamingGroupInvitation gamingGroupInvitation = new GamingGroupInvitation
            {
                DateSent = DateTime.UtcNow,
                GamingGroupId = currentUser.CurrentGamingGroupId.Value,
                InviteeEmail = playerInvitation.InvitedPlayerEmail,
                InvitingUserId = currentUser.Id,
                PlayerId = playerInvitation.InvitedPlayerId
            };

            GamingGroupInvitation savedGamingGroupInvitation = dataContext.Save<GamingGroupInvitation>(gamingGroupInvitation, currentUser);
            //commit so we can get the Id back
            dataContext.CommitAllChanges();

            string messageBody = string.Format(PlayerInviter.EMAIL_MESSAGE_INVITE_PLAYER,
                                                currentUser.UserName,
                                                gamingGroup.Name,
                                                "http://nerdscorekeeper.azurewebsites.net",
                                                gamingGroup.Id,
                                                playerInvitation.CustomEmailMessage,
                                                savedGamingGroupInvitation.Id);
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
