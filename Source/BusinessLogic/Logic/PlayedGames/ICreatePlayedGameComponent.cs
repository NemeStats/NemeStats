using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.PlayedGames
{
    public interface ICreatePlayedGameComponent
    {
        PlayedGame Execute(NewlyCompletedGame newlyCompletedGame, ApplicationUser currentUser);

        PlayedGame ExecuteTransaction(NewlyCompletedGame newlyCompletedGame, ApplicationUser currentUser, IDataContext dataContext);
    }
}