using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Logic.Utilities
{
    public static class WinPercentageCalculator
    {
        public static int CalculateWinPercentage(int gamesWon, int gamesLost)
        {
            if (gamesLost + gamesWon == 0)
            {
                return 0;
            }
            return (int)((decimal)gamesWon / (gamesLost + gamesWon) * 100);
        }
    }
}
