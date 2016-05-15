using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Points;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class HardcoreAchievement : IAchievement
    {
        public const int THREE_HOURS_WORTH_OF_MINUTES = 60 * 3;

        public AchievementId Id => AchievementId.Revenge;
        public AchievementGroup Group => AchievementGroup.Game;
        public string Name => "Hardcore";
        public string Description => "Plays different games that are both extreme in Board Game Geek Average Play Time and Board Game Geek Average Weight";
        public string IconClass => "fa fa-hand-rock-o";

        public Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 1},
            {AchievementLevel.Silver, 5},
            {AchievementLevel.Gold, 20}
        };

        public AchievementAwarded IsAwardedForThisPlayer(int playerId, IDataContext dataContext)
        {
            var result = new AchievementAwarded
            {
                AchievementId = Id
            };

            var totalDistinctHardcoreGamesThatTakeALongTime =
                dataContext
                    .GetQueryable<PlayerGameResult>()
                    .Where(x => x.PlayerId == playerId
                                && (((x.PlayedGame.GameDefinition.BoardGameGeekGameDefinition.MinPlayTime ?? 0
                                        + x.PlayedGame.GameDefinition.BoardGameGeekGameDefinition.MaxPlayTime ?? 0) / 2) > THREE_HOURS_WORTH_OF_MINUTES)
                                && x.PlayedGame.GameDefinition.BoardGameGeekGameDefinition.AverageWeight
                                >= WeightTierCalculator.BOARD_GAME_GEEK_WEIGHT_INCLUSIVE_LOWER_BOUND_FOR_HARDCORE)
                    .Select(x => x.PlayedGame.GameDefinitionId)
                    .Distinct()
                    .Count();

            if (totalDistinctHardcoreGamesThatTakeALongTime < LevelThresholds[AchievementLevel.Bronze])
            {
                return result;
            }

            result.LevelAwarded =
                LevelThresholds.OrderByDescending(l => l.Value)
                    .FirstOrDefault(l => l.Value <= totalDistinctHardcoreGamesThatTakeALongTime)
                    .Key;
            return result;
        }
    }
}
