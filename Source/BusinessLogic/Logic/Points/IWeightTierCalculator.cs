using BusinessLogic.Models.PlayedGames;

namespace BusinessLogic.Logic.Points
{
    public interface IWeightTierCalculator
    {
        WeightTierEnum GetWeightTier(decimal? boardGameGeekAverageWeight);
    }
}
