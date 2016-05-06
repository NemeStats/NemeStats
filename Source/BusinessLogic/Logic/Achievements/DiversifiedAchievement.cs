using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class DiversifiedAchievement : IAchievement
    {
        public AchievementType AchievementType => AchievementType.Diversified;

        public Dictionary<AchievementLevelEnum, int> LevelThresholds => new Dictionary<AchievementLevelEnum, int>
        {
            {AchievementLevelEnum.Bronze, 5},
            {AchievementLevelEnum.Silver, 25},
            {AchievementLevelEnum.Gold, 100}
        };

        public AchievementLevelEnum? AchievementLevelAwarded(Player player)
        {
            var differentPlayedGamesCount = player.PlayerGameResults.Select(pgr => pgr.PlayedGame.GameDefinition.Id).Distinct().Count();

            return LevelThresholds.OrderBy(l => l.Value).FirstOrDefault(l => l.Value <= differentPlayedGamesCount).Key;            
        }
    }
}