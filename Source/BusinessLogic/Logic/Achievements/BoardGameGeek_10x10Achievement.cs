using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public abstract class BoardGameGeek_10x10Achievement : BaseAchievement
    {
        protected BoardGameGeek_10x10Achievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementGroup Group => AchievementGroup.Game;

        public abstract int Year { get; }
        public override string Name => $"BoardGameGeek {Year} 10x10 Challenge";
        public override string IconClass => "ns-icon-bgg";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Gold, 10}
        };        

        public override AchievementAwarded IsAwardedForThisPlayer(int playerId)
        {
            var result = new AchievementAwarded
            {
                AchievementId = Id
            };
            var startDate = new DateTime(Year, 1, 1);
            var endDate = new DateTime(Year, 12, 31);

            var numberOfGamesWith10PlaysInYear =
                DataContext
                    .GetQueryable<PlayerGameResult>()
                    .Where(
                        x =>
                            x.PlayerId == playerId && x.PlayedGame.DatePlayed >= startDate &&
                            x.PlayedGame.DatePlayed <= endDate)
                    .GroupBy(x => x.PlayedGame.GameDefinitionId)
                    .Select(group => new { group.Key, Count = group.Count() })
                    .Where(x => x.Count >= 10)
                    .Select(s => s.Key)
                    .ToList();

            result.PlayerProgress = numberOfGamesWith10PlaysInYear.Count;
            if (result.PlayerProgress == LevelThresholds[AchievementLevel.Gold])
            {
                result.RelatedEntities = numberOfGamesWith10PlaysInYear;
                result.LevelAwarded = AchievementLevel.Gold;
            }
            return result;
        }
    }
}