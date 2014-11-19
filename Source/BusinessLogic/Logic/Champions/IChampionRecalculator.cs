using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Models;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.Champions
{
    public interface IChampionRecalculator
    {
        void RecalculateAllChampions();
        Champion RecalculateChampion(int gameDefinitionId, ApplicationUser applicationUser);
    }
}
