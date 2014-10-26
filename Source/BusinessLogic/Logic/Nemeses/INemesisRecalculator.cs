using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System.Linq;

namespace BusinessLogic.Logic.Nemeses
{
    public interface INemesisRecalculator
    {
        void RecalculateAllNemeses();
        Nemesis RecalculateNemesis(int playerId, ApplicationUser currentUser);
    }
}
