using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Models.PlayedGames;

namespace BusinessLogic.Logic.PlayedGames
{
    public class WinnerTypeCalculator : IWinnerTypeCalculator
    {
        public WinnerTypes CalculateWinnerType(IList<int> gameRanks)
        {
            var winnerType = WinnerTypes.PlayerWin;

            if (gameRanks.All(rank => rank == 1))
            {
                winnerType = WinnerTypes.TeamWin;
            }
            else if (gameRanks.All(rank => rank > 1))
            {
                winnerType = WinnerTypes.TeamLoss;
            }

            return winnerType;
        }
    }
}