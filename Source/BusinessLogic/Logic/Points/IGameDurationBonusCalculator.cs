using System.Collections.Generic;
using BusinessLogic.Models.PlayedGames;

namespace BusinessLogic.Logic.Points
{
    public interface IGameDurationBonusCalculator
    {
        void CalculateGameDurationBonus(Dictionary<int, PointsScorecard> scorecardsWithBasePoints, int? boardGameGeekAvergePlayTime);
    }
}
