#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion
using System;
using System.Configuration.Abstractions;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Exceptions;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using Microsoft.AspNet.Identity;

namespace BusinessLogic.Logic.Players
{
    public class PlayerInviter : IPlayerInviter
    {
        internal const string APP_SETTING_URL_ROOT = "urlRoot";

        private readonly IDataContext _dataContext;
        private readonly IIdentityMessageService _emailService;
        private readonly IConfigurationManager _configurationManager;

        public PlayerInviter(IDataContext dataContext, IIdentityMessageService emailService, IConfigurationManager configurationManager)
        {
            _dataContext = dataContext;
            _emailService = emailService;
            _configurationManager = configurationManager;
        }

        public void InvitePlayer(PlayerInvitation playerInvitation, ApplicationUser currentUser)
        {
            var gamingGroup = _dataContext.FindById<GamingGroup>(playerInvitation.GamingGroupId);

            var existingUserId = (from ApplicationUser user in _dataContext.GetQueryable<ApplicationUser>()
                                     where user.Email == playerInvitation.InvitedPlayerEmail
                                     select user.Id).FirstOrDefault();       
            
            var gamingGroupInvitation = new GamingGroupInvitation
            {
                DateSent = DateTime.UtcNow,
                GamingGroupId = playerInvitation.GamingGroupId,
                InviteeEmail = playerInvitation.InvitedPlayerEmail,
                InvitingUserId = currentUser.Id,
                PlayerId = playerInvitation.InvitedPlayerId,
                RegisteredUserId = existingUserId
            };

            var savedGamingGroupInvitation = _dataContext.Save(gamingGroupInvitation, currentUser);
            //commit so we can get the Id back
            _dataContext.CommitAllChanges();

            var urlRoot = _configurationManager.AppSettings[APP_SETTING_URL_ROOT];

            var customMessage = string.Empty;
            if (!string.IsNullOrWhiteSpace(playerInvitation.CustomEmailMessage))
            {
                customMessage = $"{currentUser.UserName} says: {playerInvitation.CustomEmailMessage}";
            }

            var messageBody = $@"Well hello there! You've been invited by '{currentUser.UserName}' to join the NemeStats Gaming Group called '{gamingGroup.Name}'! 
                To join this Gaming Group, click on the following link: {urlRoot}/Account/ConsumeInvitation/{savedGamingGroupInvitation.Id} <br/><br/>

                {customMessage} <br/><br/>

                If you believe you've received this in error just disregard the email.";

            var message = new IdentityMessage
            {
                Body = messageBody,
                Destination = playerInvitation.InvitedPlayerEmail,
                Subject = playerInvitation.EmailSubject
            };
            
            _emailService.SendAsync(message);
        }
    }
}
