using System;
using System.Collections.Generic;
using BusinessLogic.Models.PlayedGames;

namespace BusinessLogic.Logic.Points
{
    public class GameDurationBonusCalculator : IGameDurationBonusCalculator
    {
        internal const int MINUTES_PER_HALF_HOUR = 30;

        public void CalculateGameDurationBonus(Dictionary<int, PointsScorecard> scorecardsWithBasePoints, int? boardGameGeekAvergePlayTime)
        {
            var multiplier = GetMultiplier(boardGameGeekAvergePlayTime);

            ApplyGameDurationBonusToEachScorecard(scorecardsWithBasePoints, multiplier);
        }

        private static decimal GetMultiplier(int? boardGameGeekAvergePlayTime)
        {
            int numberOfHalfHourPeriods = 0;
            if (boardGameGeekAvergePlayTime.HasValue)
            {
                numberOfHalfHourPeriods = boardGameGeekAvergePlayTime.Value / MINUTES_PER_HALF_HOUR;
            }

            decimal multiplier = 0M;
            switch (numberOfHalfHourPeriods)
            {
                case 0:
                    multiplier = -.5M;
                    break;
                case 1:
                    multiplier = 0M;
                    break;
                case 2:
                    multiplier = 1M;
                    break;
                case 3:
                    multiplier = 1.9M;
                    break;
                case 4:
                    multiplier = 2.7M;
                    break;
                case 5:
                    multiplier = 3.4M;
                    break;
                case 6:
                    multiplier = 4M;
                    break;
                case 7:
                    multiplier = 4.5M;
                    break;
                case 8:
                    multiplier = 4.9M;
                    break;
                case 9:
                    multiplier = 5.2M;
                    break;
                case 10:
                    multiplier = 5.4M;
                    break;
                case 11:
                    multiplier = 5.5M;
                    break;
                default:
                    multiplier = 5.5M;
                    break;
            }
            return multiplier;
        }

        private static void ApplyGameDurationBonusToEachScorecard(Dictionary<int, PointsScorecard> scorecardsWithBasePoints, decimal multiplier)
        {
            foreach (var scorecard in scorecardsWithBasePoints)
            {
                scorecard.Value.GameDurationBonusPoints = (int)Math.Ceiling(multiplier * scorecard.Value.BasePoints);
            }
        }
    }
}