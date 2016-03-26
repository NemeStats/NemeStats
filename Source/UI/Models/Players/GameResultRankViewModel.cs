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
                var result = $"{GameResult.NemePointsSummary.BaseNemePoints} base points";
                if (GameResult.NemePointsSummary.WeightBonusNemePoints != 0)
                {
                    result = result + $" + {GameResult.NemePointsSummary.WeightBonusNemePoints} bonus for game weight";
                }
                if (GameResult.NemePointsSummary.GameDurationBonusNemePoints > 0)
                {
                    result = result + $" + {GameResult.NemePointsSummary.GameDurationBonusNemePoints} bonus for game duration";
                }
                if (GameResult.NemePointsSummary.GameDurationBonusNemePoints < 0)
                {
                    result = result + $" {GameResult.NemePointsSummary.GameDurationBonusNemePoints} penalty for game duration";
                }

                return result;
            }
        }

        public string Id => $"{GameResult.PlayedGameId}_{GameResult.PlayerId}";
    }
}