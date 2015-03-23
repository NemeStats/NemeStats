using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Models.User
{
    public class NewlyRegisteredUser
    {
        public int UserId { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int GamingGroupId { get; set; }
        public string GamingGroupName { get; set; }
        public string AuthenticationToken { get; set; }
    }
}
