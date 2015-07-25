using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.Users
{
    public interface IUserRetriever
    {
        UserInformation RetrieveUserInformation(string expectedUserID, ApplicationUser applicationUser);
    }
}
