using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Points;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.PlayedGames;

namespace BusinessLogic.Logic.Achievements
{
    public class HardcoreAchievement : BaseAchievement
    {
        public HardcoreAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public const int THREE_HOURS_WORTH_OF_MINUTES = 60 * 3;

        public override AchievementId Id => AchievementId.Hardcore;
        public override AchievementGroup Group => AchievementGroup.Game;
        public override string Name => "Hardcore";

        public override string Description => @"This Achievement is earned by playing Games that have a BoardGameGeek average play time of at least 3 hours, and an
                                              average weight of at least " + WeightTierCalculator.BOARD_GAME_GEEK_WEIGHT_INCLUSIVE_LOWER_BOUND_FOR_HARDCORE
                                              + " (Hardcore).";

        public override string IconClass => "fa fa-hand-rock-o";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 1},
            {AchievementLevel.Silver, 5},
            {AchievementLevel.Gold, 20}
        };

        public override AchievementAwarded IsAwardedForThisPlayer(int playerId)
        {
            var result = new AchievementAwarded
            {
                AchievementId = Id
            };

            var totalDistinctHardcoreGamesThatTakeALongTime =
                DataContext
                    .GetQueryable<PlayerGameResult>()
                    .Where(x => x.PlayerId == playerId
                                && ((((x.PlayedGame.GameDefinition.BoardGameGeekGameDefinition.MinPlayTime ?? 0)
                                        + (x.PlayedGame.GameDefinition.BoardGameGeekGameDefinition.MaxPlayTime ?? 0)) / 2) >= THREE_HOURS_WORTH_OF_MINUTES)
                                && x.PlayedGame.GameDefinition.BoardGameGeekGameDefinition.AverageWeight
                                >= WeightTierCalculator.BOARD_GAME_GEEK_WEIGHT_INCLUSIVE_LOWER_BOUND_FOR_HARDCORE)
                    .Select(x => x.PlayedGame.GameDefinitionId)
                    .Distinct()
                    .Count();

            result.PlayerProgress = totalDistinctHardcoreGamesThatTakeALongTime;
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
