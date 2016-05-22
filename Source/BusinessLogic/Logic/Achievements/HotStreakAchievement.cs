using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class HotStreakAchievement : BaseAchievement
    {
        private readonly EntityFrameworkPlayerRepository _entityFrameworkPlayerRepository;

        public HotStreakAchievement(IDataContext dataContext, EntityFrameworkPlayerRepository entityFrameworkPlayerRepository) : base(dataContext)
        {
            _entityFrameworkPlayerRepository = entityFrameworkPlayerRepository;
        }

        public override AchievementId Id => AchievementId.HotStreak;

        public override AchievementGroup Group => AchievementGroup.Game;

        public override string Name => "Hot Streak";

        public override string DescriptionFormat => "This Achievement is earned by winning {0} games consecutively.";

        public override string IconClass => "fa fa-fire";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 5},
            {AchievementLevel.Silver, 9},
            {AchievementLevel.Gold, 15}
        };

        public override AchievementAwarded IsAwardedForThisPlayer(int playerId)
        {
            
            var result = new AchievementAwarded
            {
                AchievementId = Id
            };

            var longestWinStreak = _entityFrameworkPlayerRepository.GetLongestWinningStreak(playerId);
            result.PlayerProgress = longestWinStreak;

            if (longestWinStreak < LevelThresholds[AchievementLevel.Bronze])
            {
                return result;
            }

            result.LevelAwarded = GetLevelAwarded(longestWinStreak);
            return result;
        }
    }
}
