using Microsoft.AspNet.Identity;
using SendGrid;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

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
            myMessage.From = new System.Net.Mail.MailAddress(
                                "nemestats@gmail.com", "NemeStats");
            myMessage.Subject = message.Subject;
            myMessage.Text = message.Body;
            myMessage.Html = message.Body;

            var credentials = new NetworkCredential(
                       ConfigurationManager.AppSettings["emailServiceUserName"],
                       ConfigurationManager.AppSettings["emailServicePassword"]
                       );

            // Create a Web transport for sending email.
            var transportWeb = new Web(credentials);

            // Send the email.
            return transportWeb.DeliverAsync(myMessage);
        }
    }
}
