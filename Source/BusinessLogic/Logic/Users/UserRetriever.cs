using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.Users
{
    public class UserRetriever : IUserRetriever
    {
        private readonly IDataContext dataContext;

        public UserRetriever(IDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public UserInformation RetrieveUserInformation(string userId, ApplicationUser applicationUser)
        {
            if (userId != applicationUser.Id)
            {
                throw new UnauthorizedEntityAccessException(applicationUser.Id, typeof(ApplicationUser), userId);
            }

            return dataContext.GetQueryable<ApplicationUser>()
                              .Where(user => user.Id == userId)
                              .Select(user => new UserInformation
                              {
                                UserId = user.Id,
                                Email = user.Email,
                                UserName = user.UserName,
                                GamingGroups = user.UserGamingGroups.Select(userGamingGroup => new GamingGroupInfoForUser
                                {
                                    GamingGroupId = userGamingGroup.GamingGroup.Id,
                                    GamingGroupName = userGamingGroup.GamingGroup.Name
                                }).ToList(),
                                Players = user.Players.Select(player => new PlayerInfoForUser
                                {
                                    PlayerId = player.Id,
                                    PlayerName = player.Name
                                }).ToList()
                              }).First();
        }
    }
}
