using BusinessLogic.Models.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.Points
{
    public static class GordonPoints
    {
        internal static int CalculateGordonPoints(int numberOfPlayers, int gameRank)
        {
            return numberOfPlayers - gameRank + 1;
        }
    }
}
