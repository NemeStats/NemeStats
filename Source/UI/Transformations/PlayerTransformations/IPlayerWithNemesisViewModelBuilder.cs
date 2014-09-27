using BusinessLogic.Models;
using System;
using UI.Views.Player;
namespace UI.Transformations.PlayerTransformations
{
    public interface IPlayerWithNemesisViewModelBuilder
    {
        PlayerWithNemesisViewModel Build(Player player);
    }
}
