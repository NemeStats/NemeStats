using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI.Areas.Api.Models
{
    public class UserInformationMessage
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public IList<UserGamingGroupInfo> GamingGroups { get; set; }
        public IList<PlayersForUserInfo> Players { get; set; }
    }
}
