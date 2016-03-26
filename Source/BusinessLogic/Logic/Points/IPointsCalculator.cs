using System.Collections.Generic;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.PlayedGames;

namespace BusinessLogic.Logic.Points
{
    public interface IPointsCalculator
    {
        Dictionary<int, PointsScorecard> CalculatePoints(IList<PlayerRank> playerRanks, BoardGameGeekGameDefinition bggGameDefinition);
    }
}