using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class ChampionAchievement : IAchievement
    {
        public AchievementType AchievementType => AchievementType.Champion;

        public Dictionary<AchievementLevelEnum, int> LevelThresholds => new Dictionary<AchievementLevelEnum, int>
        {
            {AchievementLevelEnum.Bronze, 1},
            {AchievementLevelEnum.Silver, 10},
            {AchievementLevelEnum.Gold, 50}
        };

        public AchievementLevelEnum? AchievementLevelAwarded(int playerId, IDataContext dataContext)
        {
            var championedGames = dataContext.GetQueryable<Champion>().Count(c=>c.PlayerId == playerId);

            return LevelThresholds.OrderByDescending(l => l.Value).FirstOrDefault(l => l.Value <= championedGames).Key;
        }
    }
}