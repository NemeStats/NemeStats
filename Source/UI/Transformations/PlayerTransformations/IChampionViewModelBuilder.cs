using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Models;
using UI.Models;

namespace UI.Transformations.PlayerTransformations
{
    public interface IChampionViewModelBuilder
    {
        ChampionViewModel Build(Champion champion);
    }
}
