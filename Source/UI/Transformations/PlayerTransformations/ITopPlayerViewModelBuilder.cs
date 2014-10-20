using BusinessLogic.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Models.Players;

namespace UI.Transformations.PlayerTransformations
{
    public interface ITopPlayerViewModelBuilder
    {
        TopPlayerViewModel Build(TopPlayer topPlayer);
    }
}
