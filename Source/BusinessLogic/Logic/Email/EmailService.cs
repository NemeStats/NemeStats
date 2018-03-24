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
using Microsoft.AspNet.Identity;
using SendGrid;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;

namespace BusinessLogic.Logic.Email
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            return configSendGridasync(message);
        }

        private Task configSendGridasync(IdentityMessage message)
        {
            var myMessage = new SendGridMessage();
            myMessage.AddTo(message.Destination);
            myMessage.From = new EmailAddress("nemestats@gmail.com", "NemeStats");
            myMessage.Subject = message.Subject;
            myMessage.HtmlContent = message.Body;
            myMessage.PlainTextContent = message.Body;

            //TODO REMOVE THIS?
            //var credentials = new NetworkCredential(
            //           ConfigurationManager.AppSettings["emailServiceUserName"],
            //           ConfigurationManager.AppSettings["emailServicePassword"]
            //           );

            var apiKey = ConfigurationManager.AppSettings["sendgridApiKey"];
            var sendGridClient = new SendGridClient(apiKey);

            // Send the email.
            return sendGridClient.SendEmailAsync(myMessage);
        }
    }
}
