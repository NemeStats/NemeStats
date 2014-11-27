using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLogic.Models;
using UI.Models.Players;

namespace UI.Transformations.PlayerTransformations
{
    public interface IPlayerEditViewModelBuilder
    {
        PlayerEditViewModel Build(Player player);
    }
}
