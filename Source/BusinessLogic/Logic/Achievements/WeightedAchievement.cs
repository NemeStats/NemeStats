using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Logic.Points;

namespace BusinessLogic.Logic.Achievements
{
    public class WeightedAchievement : BaseAchievement
    {
        public WeightedAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.Weighted;

        public override AchievementGroup Group => AchievementGroup.Game;

        public override string Name => "Weighted!";

        public override string DescriptionFormat => "This Achievement is earned by playing {0} games of each weight (Easy, Hardcore, etc).";

        public override string IconClass => "fa fa-balance-scale";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 1},
            {AchievementLevel.Silver, 5},
            {AchievementLevel.Gold, 15}
        };

        public override AchievementAwarded IsAwardedForThisPlayer(int playerId)
        {
            var result = new AchievementAwarded
            {
                AchievementId = Id
            };

            var allWeightedGames =
                DataContext
                    .GetQueryable<PlayerGameResult>()
                    .Where(x => x.PlayerId == playerId)
                    .GroupBy(x => new {
                        WeightTier = x.PlayedGame.GameDefinition.BoardGameGeekGameDefinition.AverageWeight
                                        < WeightTierCalculator.BOARD_GAME_GEEK_WEIGHT_INCLUSIVE_LOWER_BOUND_FOR_EASY ? WeightTierEnum.Casual :
                                        x.PlayedGame.GameDefinition.BoardGameGeekGameDefinition.AverageWeight
                                        < WeightTierCalculator.BOARD_GAME_GEEK_WEIGHT_INCLUSIVE_LOWER_BOUND_FOR_ADVANCED ? WeightTierEnum.Easy :
                                        x.PlayedGame.GameDefinition.BoardGameGeekGameDefinition.AverageWeight
                                        < WeightTierCalculator.BOARD_GAME_GEEK_WEIGHT_INCLUSIVE_LOWER_BOUND_FOR_CHALLENGING ? WeightTierEnum.Challenging :
                                        x.PlayedGame.GameDefinition.BoardGameGeekGameDefinition.AverageWeight
                                        < WeightTierCalculator.BOARD_GAME_GEEK_WEIGHT_INCLUSIVE_LOWER_BOUND_FOR_HARDCORE ? WeightTierEnum.Advanced :
                                        WeightTierEnum.Hardcore
                    })
                    .Select(group => new { group.Key, Count = group.Count() })
                    .ToList();

            var noUnknownGames =
                (from item in allWeightedGames
                    where item.Key.WeightTier != WeightTierEnum.Unknown
                    select item).ToList();
                

            if (noUnknownGames.Count == 5)
            {
                result.PlayerProgress = noUnknownGames.Min(p => p.Count);
            }
            else
            {
                return result;
            }

            result.LevelAwarded = GetLevelAwarded(result.PlayerProgress);

            return result;
        }
    }
}
