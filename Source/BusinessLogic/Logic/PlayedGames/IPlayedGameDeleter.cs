using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.PlayedGames
{
    public interface IPlayedGameDeleter
    {
        void DeletePlayedGame(int playedGameId, ApplicationUser currentUser);
    }
}
