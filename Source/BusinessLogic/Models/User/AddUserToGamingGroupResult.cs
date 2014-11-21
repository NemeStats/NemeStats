using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Models.User
{
    public class AddUserToGamingGroupResult
    {
        public bool UserAddedToExistingGamingGroup { get; set; }

        public string EmailAddress { get; set; }
    }
}
