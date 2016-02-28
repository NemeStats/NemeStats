using BusinessLogic.Models.PlayedGames;

namespace BusinessLogic.Logic.Points
{
    public class AverageGameDurationTierCalculator : IAverageGameDurationTierCalculator
    {
        public const decimal BOARD_GAME_GEEK_PLAYING_TIME_INCLUSIVE_LOWER_BOUND_FOR_SHORT = 30;
        public const decimal BOARD_GAME_GEEK_PLAYING_TIME_INCLUSIVE_LOWER_BOUND_FOR_MEDIUM = 60;
        public const decimal BOARD_GAME_GEEK_PLAYING_TIME_INCLUSIVE_LOWER_BOUND_FOR_LONG = 90;
        public const decimal BOARD_GAME_GEEK_PLAYING_TIME_INCLUSIVE_LOWER_BOUND_FOR_VERY_LONG = 120;

        public AverageGameDurationTierEnum GetAverageGameDurationTier(int? playingTime)
        {

            if (!playingTime.HasValue || playingTime == 0)
            {
                return AverageGameDurationTierEnum.Unknown;
            }
            var time = playingTime.Value;
            if (time < BOARD_GAME_GEEK_PLAYING_TIME_INCLUSIVE_LOWER_BOUND_FOR_SHORT)
            {
                return AverageGameDurationTierEnum.VeryShort;
            }
            if (time < BOARD_GAME_GEEK_PLAYING_TIME_INCLUSIVE_LOWER_BOUND_FOR_MEDIUM)
            {
                return AverageGameDurationTierEnum.Short;
            }
            if (time < BOARD_GAME_GEEK_PLAYING_TIME_INCLUSIVE_LOWER_BOUND_FOR_LONG)
            {
                return AverageGameDurationTierEnum.Medium;
            }
            if (time < BOARD_GAME_GEEK_PLAYING_TIME_INCLUSIVE_LOWER_BOUND_FOR_VERY_LONG)
            {
                return AverageGameDurationTierEnum.Long;
            }
            return AverageGameDurationTierEnum.VeryLong;
        }
    }
}
