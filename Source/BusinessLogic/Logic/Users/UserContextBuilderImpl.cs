using BusinessLogic.DataAccess;
using BusinessLogic.Models.User;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Logic.Users
{
    public class UserContextBuilderImpl : UserContextBuilder
    {
        internal const string EXCEPTION_MESSAGE_NO_CURRENT_GAMING_GROUP = "No CurrentGamingGroupId was found on user Id {0} with UserName {1}";
        internal const string EXCEPTION_MESSAGE_USER_NOT_FOUND
            = "User with Id '{0}' not found.";

        public UserContext GetUserContext(string userId, NemeStatsDbContext dbContext)
        {
            ApplicationUser applicationUser = dbContext.Users.Where(user => user.Id == userId).FirstOrDefault();

            if(applicationUser == null)
            {
                string message = string.Format(EXCEPTION_MESSAGE_USER_NOT_FOUND, userId);
                throw new KeyNotFoundException(message);
            }

            UserContext userContext = new UserContext()
            {
                ApplicationUserId = applicationUser.Id,
                GamingGroupId = applicationUser.CurrentGamingGroupId
            };
            
            return userContext;
        }
    }
}
