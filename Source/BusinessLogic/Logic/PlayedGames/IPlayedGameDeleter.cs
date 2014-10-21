using BusinessLogic.Models.User;
using System.Linq;

namespace BusinessLogic.Logic.PlayedGames
{
    public interface IPlayedGameDeleter
    {
        void DeletePlayedGame(int playedGameId, ApplicationUser currentUser);
    }
}
