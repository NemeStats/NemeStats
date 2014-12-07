using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using System.Linq;
using UI.Models.Players;

namespace UI.Transformations.PlayerTransformations
{
    public interface IPlayerDetailsViewModelBuilder
    {
        PlayerDetailsViewModel Build(PlayerDetails playerDetails, string urlToMinionsAnchor, ApplicationUser currentUser = null);
    }
}
