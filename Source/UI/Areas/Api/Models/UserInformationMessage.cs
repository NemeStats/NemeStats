using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI.Areas.Api.Models
{
    public class UserInformationMessage
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public IList<GamingGroupInfoForUserMessage> GamingGroups { get; set; }
        public IList<PlayerInfoForUserMessage> Players { get; set; }
    }
}
