using BusinessLogic.DataAccess;
using BusinessLogic.Models.User;
using System.Linq;

namespace BusinessLogic.Logic.Users
{
    public class UserContextBuilderImpl : UserContextBuilder
    {
        internal const string EXCEPTION_MESSAGE_NO_CURRENT_GAMING_GROUP = "No CurrentGamingGroupId was found on user Id {0} with UserName {1}";

        public UserContext GetUserContext(string userId, NemeStatsDbContext dbContext)
        {
            ApplicationUser applicationUser = dbContext.Users.Where(user => user.Id == userId).FirstOrDefault();

            UserContext userContext = new UserContext()
            {
                ApplicationUserId = applicationUser.Id,
                GamingGroupId = applicationUser.CurrentGamingGroupId
            };
            
            return userContext;
        }
    }
}
