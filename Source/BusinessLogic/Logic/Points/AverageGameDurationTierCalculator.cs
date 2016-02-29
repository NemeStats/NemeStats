using BusinessLogic.Models.PlayedGames;

namespace BusinessLogic.Logic.Points
{
    public class AverageGameDurationTierCalculator : IAverageGameDurationTierCalculator
    {
        public const decimal BOARD_GAME_GEEK_PLAYING_TIME_INCLUSIVE_LOWER_BOUND_FOR_SHORT = 30;
        public const decimal BOARD_GAME_GEEK_PLAYING_TIME_INCLUSIVE_LOWER_BOUND_FOR_MEDIUM = 60;
        public const decimal BOARD_GAME_GEEK_PLAYING_TIME_INCLUSIVE_LOWER_BOUND_FOR_LONG = 120;
        public const decimal BOARD_GAME_GEEK_PLAYING_TIME_INCLUSIVE_LOWER_BOUND_FOR_VERY_LONG = 200;
        public const decimal BOARD_GAME_GEEK_PLAYING_TIME_INCLUSIVE_LOWER_BOUND_FOR_RIDICULOUSLY_LONG = 300;

        public AverageGameDurationTierEnum GetAverageGameDurationTier(int? playingTime)
        {
            if (!playingTime.HasValue || playingTime == 0)
            {
                return AverageGameDurationTierEnum.Unknown;
            }
            var weight = playingTime.Value;
            if (weight < BOARD_GAME_GEEK_PLAYING_TIME_INCLUSIVE_LOWER_BOUND_FOR_SHORT)
            {
                return AverageGameDurationTierEnum.VeryShort;
            }
            if (weight < BOARD_GAME_GEEK_PLAYING_TIME_INCLUSIVE_LOWER_BOUND_FOR_MEDIUM)
            {
                return AverageGameDurationTierEnum.Short;
            }
            if (weight < BOARD_GAME_GEEK_PLAYING_TIME_INCLUSIVE_LOWER_BOUND_FOR_LONG)
            {
                return AverageGameDurationTierEnum.Medium;
            }
            if (weight < BOARD_GAME_GEEK_PLAYING_TIME_INCLUSIVE_LOWER_BOUND_FOR_VERY_LONG)
            {
                return AverageGameDurationTierEnum.Long;
            }
            if (weight < BOARD_GAME_GEEK_PLAYING_TIME_INCLUSIVE_LOWER_BOUND_FOR_RIDICULOUSLY_LONG)
            {
                return AverageGameDurationTierEnum.VeryLong;
            }
            return AverageGameDurationTierEnum.RidiculouslyLong;
        }
    }
}
