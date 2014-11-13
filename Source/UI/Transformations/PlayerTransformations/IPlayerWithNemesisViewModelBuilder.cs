using BusinessLogic.Models;
using BusinessLogic.Models.User;
using UI.Models.Players;
namespace UI.Transformations.PlayerTransformations
{
    public interface IPlayerWithNemesisViewModelBuilder
    {
        PlayerWithNemesisViewModel Build(Player player, ApplicationUser currentUser);
    }
}
