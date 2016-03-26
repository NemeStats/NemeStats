using BusinessLogic.Models.PlayedGames;

namespace BusinessLogic.Logic.Points
{
    public class WeightTierCalculator : IWeightTierCalculator
    {
        public const decimal BOARD_GAME_GEEK_WEIGHT_INCLUSIVE_LOWER_BOUND_FOR_EASY = (decimal)1.8;
        public const decimal BOARD_GAME_GEEK_WEIGHT_INCLUSIVE_LOWER_BOUND_FOR_ADVANCED = (decimal)2.4;
        public const decimal BOARD_GAME_GEEK_WEIGHT_INCLUSIVE_LOWER_BOUND_FOR_CHALLENGING = (decimal)3.3;
        public const decimal BOARD_GAME_GEEK_WEIGHT_INCLUSIVE_LOWER_BOUND_FOR_HARDCORE = (decimal)4.1;

        public WeightTierEnum GetWeightTier(decimal? boardGameGeekAverageWeight)
        {
            if (!boardGameGeekAverageWeight.HasValue || boardGameGeekAverageWeight == 0)
            {
                return WeightTierEnum.Unknown;
            }
            var weight = boardGameGeekAverageWeight.Value;
            if (weight < BOARD_GAME_GEEK_WEIGHT_INCLUSIVE_LOWER_BOUND_FOR_EASY)
            {
                return WeightTierEnum.Casual;
            }
            if (weight < BOARD_GAME_GEEK_WEIGHT_INCLUSIVE_LOWER_BOUND_FOR_ADVANCED)
            {
                return WeightTierEnum.Easy;
            }
            if (weight < BOARD_GAME_GEEK_WEIGHT_INCLUSIVE_LOWER_BOUND_FOR_CHALLENGING)
            {
                return WeightTierEnum.Advanced;
            }
            if (weight < BOARD_GAME_GEEK_WEIGHT_INCLUSIVE_LOWER_BOUND_FOR_HARDCORE)
            {
                return WeightTierEnum.Challenging;
            }
            return WeightTierEnum.Hardcore;
        }
    }
}
