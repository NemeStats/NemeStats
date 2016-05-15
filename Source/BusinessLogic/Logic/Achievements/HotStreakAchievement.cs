using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class HotStreakAchievement : IAchievement
    {
        public AchievementId Id => AchievementId.HotStreak;
        public AchievementGroup Group => AchievementGroup.Game;
        public string Name => "Hot Streak";
        public string Description => "Win many games consecutively";
        public string IconClass => "fa fa-fire";

        public Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 5},
            {AchievementLevel.Silver, 9},
            {AchievementLevel.Gold, 15}
        };

        public AchievementAwarded IsAwardedForThisPlayer(int playerId, IDataContext dataContext)
        {
            var playerRepository = new EntityFrameworkPlayerRepository(dataContext);
            var result = new AchievementAwarded
            {
                AchievementId = Id
            };

            var longestWinStreak = playerRepository.GetLongestWinningStreak(playerId);

            if (longestWinStreak < LevelThresholds[AchievementLevel.Bronze])
            {
                return result;
            }

            result.LevelAwarded =
                LevelThresholds.OrderByDescending(l => l.Value)
                    .FirstOrDefault(l => l.Value <= longestWinStreak)
                    .Key;
            return result;
        }
    }
}
