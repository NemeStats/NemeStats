using BusinessLogic.DataAccess;
using BusinessLogic.Exceptions;
using BusinessLogic.Models.User;
using System.Linq;

namespace BusinessLogic.Logic
{
    public class UserContextBuilderImpl : UserContextBuilder
    {
        internal const string EXCEPTION_MESSAGE_NO_CURRENT_GAMING_GROUP = "No CurrentGamingGroupId was found on user Id {0} with UserName {1}";

        public UserContext GetUserContext(string userName, NemeStatsDbContext dbContext)
        {
            ApplicationUser applicationUser = dbContext.Users.Where(user => user.UserName == userName).FirstOrDefault();
            int? currentGamingGroup = applicationUser.CurrentGamingGroupId;

            if (currentGamingGroup == null)
            {
                string[] exceptionParameters = new string[]
                { 
                    applicationUser.Id, 
                    applicationUser.UserName
                };
                throw new NoCurrentGamingGroupException(string.Format(EXCEPTION_MESSAGE_NO_CURRENT_GAMING_GROUP, exceptionParameters));
            }

            UserContext userContext = new UserContext()
            {
                ApplicationUserId = applicationUser.Id,
                GamingGroupId = currentGamingGroup.Value
            };
            
            return userContext;
        }
    }
}
