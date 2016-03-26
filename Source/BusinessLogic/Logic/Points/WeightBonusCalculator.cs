using System;
using System.Collections.Generic;
using BusinessLogic.Models.PlayedGames;

namespace BusinessLogic.Logic.Points
{
    public class WeightBonusCalculator : IWeightBonusCalculator
    {
        private readonly IWeightTierCalculator _weightTierCalculator;
        internal const decimal BONUS_MULTIPLIER_20_PERCENT = .20M;
        internal const decimal BONUS_MULTIPLIER_30_PERCENT = .30M;

        public WeightBonusCalculator(IWeightTierCalculator weightTierCalculator)
        {
            _weightTierCalculator = weightTierCalculator;
        }

        public void CalculateWeightBonus(Dictionary<int, PointsScorecard> scorecardsWithBasePoints, decimal? bggAverageWeight)
        {
            var weightTier = _weightTierCalculator.GetWeightTier(bggAverageWeight);
            var multiplier = GetMultiplier(weightTier);

            AwardWeightBonusPointsToEachScorecard(scorecardsWithBasePoints, multiplier);
        }

        private static decimal GetMultiplier(WeightTierEnum weightTier)
        {
            decimal multiplier = 0M;
            switch (weightTier)
            {
                case WeightTierEnum.Advanced:
                case WeightTierEnum.Challenging:
                    multiplier = BONUS_MULTIPLIER_20_PERCENT;
                    break;
                case WeightTierEnum.Hardcore:
                    multiplier = BONUS_MULTIPLIER_30_PERCENT;
                    break;
            }
            return multiplier;
        }

        private static void AwardWeightBonusPointsToEachScorecard(Dictionary<int, PointsScorecard> scorecardsWithBasePoints, decimal multiplier)
        {
            foreach (var scorecard in scorecardsWithBasePoints)
            {
                scorecard.Value.GameWeightBonusPoints = (int)Math.Ceiling(multiplier * scorecard.Value.BasePoints);
            }
        }
    }
}
