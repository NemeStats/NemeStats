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
        public override string DescriptionFormat => "Completed the BoardGameGeek 2016 10x10 challenge by playing {0} games 10 times each in 2016: http://boardgamegeek.com/geeklist/201403/2016-challenge-play-10-games-10-times-each";
        public override string IconClass => "fa fa-question";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Gold, 10}
        };

        public static readonly DateTime START_OF_2016 = new DateTime(2016, 1, 1);
        public static readonly DateTime END_OF_2016 = new DateTime(2016, 12, 31);

        public override AchievementAwarded IsAwardedForThisPlayer(int playerId)
        {
            var result = new AchievementAwarded
            {
                AchievementId = Id
            };
            var numberOfGamesWith10PlaysIn2016 =
                DataContext
                    .GetQueryable<PlayerGameResult>()
                    .Where(x => x.PlayerId == playerId && x.PlayedGame.DatePlayed >= START_OF_2016 && x.PlayedGame.DatePlayed <= END_OF_2016)
                    .GroupBy(x => x.PlayedGame.GameDefinitionId)
                    .Select(group => new { group.Key, Count = group.Count()})
                    .Count(x => x.Count >= 10);

            result.PlayerProgress = numberOfGamesWith10PlaysIn2016;
            if (numberOfGamesWith10PlaysIn2016 == LevelThresholds[AchievementLevel.Gold])
            {
                result.LevelAwarded = AchievementLevel.Gold;
            }
            return result;
        }
    }
}
