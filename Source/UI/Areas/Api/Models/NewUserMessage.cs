using System.Linq;

namespace UI.Areas.Api.Models
{
    public class NewUserMessage
    {
        public string EmailAddress { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string UniqueDeviceId { get; set; }
    }
}
