using BusinessLogic.Models.PlayedGames;

namespace BusinessLogic.Logic.Points
{
    public interface IAverageGameDurationTierCalculator
    {
        AverageGameDurationTierEnum GetAverageGameDurationTier(int? playingTime);
    }
}