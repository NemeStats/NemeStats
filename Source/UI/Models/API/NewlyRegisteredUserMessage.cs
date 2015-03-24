using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI.Models.API
{
    public class NewlyRegisteredUserMessage
    {
        public string UserId { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int GamingGroupId { get; set; }
        public string GamingGroupName { get; set; }
        public string AuthenticationToken { get; set; }
    }
}
