using BusinessLogic.Models;
using System.Linq;
using UI.Models.Players;

namespace UI.Transformations.PlayerTransformations
{
    public interface IMinionViewModelBuilder
    {
        MinionViewModel Build(Player playerWithNemesis);
    }
}
