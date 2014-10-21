using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System.Linq;
using UI.Models.PlayedGame;

namespace UI.Transformations
{
    public interface IPlayedGameDetailsViewModelBuilder
    {
        PlayedGameDetailsViewModel Build(PlayedGame playedGame, ApplicationUser currentUser);
    }
}