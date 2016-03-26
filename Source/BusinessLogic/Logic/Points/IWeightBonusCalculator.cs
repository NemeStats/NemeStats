using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Models.PlayedGames;

namespace BusinessLogic.Logic.Points
{
    public interface IWeightBonusCalculator
    {
        void CalculateWeightBonus(Dictionary<int, PointsScorecard> scorecardsWithBasePoints, decimal? bggAverageWeight);
    }
}
