using System.Linq;

namespace BusinessLogic.Models.Points
{
    public static class GordonPoints
    {
        internal static int CalculateGordonPoints(int numberOfPlayers, int gameRank)
        {
            return numberOfPlayers - gameRank + 1;
        }
    }
}
