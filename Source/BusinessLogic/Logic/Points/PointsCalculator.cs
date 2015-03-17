using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Models.Games;

namespace BusinessLogic.Logic.Points
{
    public class PointsCalculator
    {
        internal const int DEFAULT_POINTS_PER_PLAYER_WHEN_EVERYONE_LOSES = 2;
        internal static Dictionary<int, int> CalculatePoints(IList<PlayerRank> playerRanks)
        {
            Dictionary<int, int> playerToPoints = new Dictionary<int, int>(playerRanks.Count);

            if (playerRanks.All(x => x.GameRank != 1))
            {
                foreach (var playerRank in playerRanks)
                {
                    playerToPoints.Add(playerRank.PlayerId, DEFAULT_POINTS_PER_PLAYER_WHEN_EVERYONE_LOSES);
                }
            }

            return playerToPoints;
        }
    }
}
