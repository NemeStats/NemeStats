
using System;

namespace UI.Areas.Api.Models
{
    public class NewlyRegisteredUserMessage
    {
        public string UserId { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int GamingGroupId { get; set; }
        public string GamingGroupName { get; set; }
        public string AuthenticationToken { get; set; }
        public DateTime? AuthenticationTokenExpirationDateTime { get; set; }
    }
}
