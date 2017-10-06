using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.GamingGroups;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.Users
{
    public class UserRetriever : IUserRetriever
    {
        private readonly IDataContext _dataContext;

        public UserRetriever(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public UserInformation RetrieveUserInformation(ApplicationUser applicationUser)
        {
           
            var userInformation = _dataContext.GetQueryable<ApplicationUser>()
                .Where(user => user.Id == applicationUser.Id)
                .Select(user => new UserInformation
                {
                    UserId = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    GamingGroups = user.UserGamingGroups.Select(userGamingGroup => new GamingGroupInfoForUser
                    {
                        GamingGroupId = userGamingGroup.GamingGroup.Id,
                        GamingGroupName = userGamingGroup.GamingGroup.Name,
                        GamingGroupPublicDescription = userGamingGroup.GamingGroup.PublicDescription,
                        GamingGroupPublicUrl = userGamingGroup.GamingGroup.PublicGamingGroupWebsite,
                        Active = userGamingGroup.GamingGroup.Active,
                        NumberOfGamesPlayed = userGamingGroup.GamingGroup.PlayedGames.Count,
                        NumberOfPlayers = userGamingGroup.GamingGroup.Players.Count,
                        GamingGroupChampion = userGamingGroup.GamingGroup.GamingGroupChampion
                    }).OrderBy(x => x.GamingGroupName).ToList(),
                    Players = user.Players.Select(player => new PlayerInfoForUser
                    {
                        PlayerId = player.Id,
                        PlayerName = player.Name,
                        GamingGroupId = player.GamingGroupId
                    }).ToList(),
                    BoardGameGeekUser = user.BoardGameGeekUser != null ? new BoardGameGeekUserInformation
                    {
                        Name = user.BoardGameGeekUser.Name,
                        Avatar = user.BoardGameGeekUser.Avatar
                    } : null
                }).First();

            return userInformation;
        }
    }
}
