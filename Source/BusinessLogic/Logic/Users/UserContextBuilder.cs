using BusinessLogic.DataAccess;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace BusinessLogic.Logic.Users
{
    public interface UserContextBuilder
    {
        UserContext GetUserContext(string userId, NemeStatsDbContext dbContext);
    }
}