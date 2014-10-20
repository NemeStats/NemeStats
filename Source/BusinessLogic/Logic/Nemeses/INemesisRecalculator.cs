using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLogic.Logic.Nemeses
{
    public interface INemesisRecalculator
    {
        void RecalculateAllNemeses();
        Nemesis RecalculateNemesis(int playerId, ApplicationUser currentUser);
    }
}
