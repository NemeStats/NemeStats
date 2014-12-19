using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using UI.Models.Players;
namespace UI.Transformations.PlayerTransformations
{
    public interface IPlayerWithNemesisViewModelBuilder
    {
        PlayerWithNemesisViewModel Build(PlayerWithNemesis playerWithNemesis, ApplicationUser currentUser);
    }
}
