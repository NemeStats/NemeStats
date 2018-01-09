using System.Collections.Generic;
using BusinessLogic.Models.PlayedGames;

namespace BusinessLogic.Logic.PlayedGames
{
    public interface IWinnerTypeCalculator
    {
        WinnerTypes CalculateWinnerType(IList<int> gameRanks);
    }
}
