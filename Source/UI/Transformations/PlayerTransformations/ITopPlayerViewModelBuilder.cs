using BusinessLogic.Models.Players;
using System.Linq;
using UI.Models.Players;

namespace UI.Transformations.PlayerTransformations
{
    public interface ITopPlayerViewModelBuilder
    {
        TopPlayerViewModel Build(TopPlayer topPlayer);
    }
}
