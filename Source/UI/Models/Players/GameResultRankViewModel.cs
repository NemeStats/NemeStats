using System;
using BusinessLogic.Models.PlayedGames;
using UI.Models.PlayedGame;

namespace UI.Models.Players
{
    public class GameResultRankViewModel
    {
        public GameResultViewModel GameResult { get; set; }
        public WinnerTypes? WinnerType { get; set; }

        public string PointsExplanation
        {
            get
            {
                var result = $"{GameResult.NemeStatsPointsAwarded} base points";
                if (GameResult.GameWeightBonusPoints != 0)
                {
                    result = result + $" + {GameResult.GameWeightBonusPoints} bonus for game weight";
                }
                if (GameResult.GameDurationBonusPoints > 0)
                {
                    result = result + $" + {GameResult.GameDurationBonusPoints} bonus for game duration";
                }
                if (GameResult.GameDurationBonusPoints < 0)
                {
                    result = result + $" {GameResult.GameDurationBonusPoints} penalty for game duration";
                }

                return result;
            }
        }

        public string Id => $"{GameResult.PlayedGameId}_{GameResult.PlayerId}";
    }
}