using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.Points;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class BoardGameGeek2016_10x10Achievement : BaseAchievement
    {
        public BoardGameGeek2016_10x10Achievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public const int THREE_HOURS_WORTH_OF_MINUTES = 60 * 3;

        public override AchievementId Id => AchievementId.BoardGameGeek2016_10x10;
        public override AchievementGroup Group => AchievementGroup.Game;
        public override string Name => "BoardGameGeek 2016 10x10 challenge";
        public override string Description => "Completed the BoardGameGeek 2016 10x10 challenge: http://boardgamegeek.com/geeklist/201403/2016-challenge-play-10-games-10-times-each";
        public override string IconClass => "fa fa-question";

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
            throw new NotImplementedException();
            var totalDistinctHardcoreGamesThatTakeALongTime =
                DataContext
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
