using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class DiversifiedAchievement : IAchievement
    {
        public AchievementType AchievementId => AchievementType.Diversified;

        public Dictionary<AchievementLevelEnum, int> LevelThresholds => new Dictionary<AchievementLevelEnum, int>
        {
            {AchievementLevelEnum.Bronze, 5},
            {AchievementLevelEnum.Silver, 25},
            {AchievementLevelEnum.Gold, 100}
        };

        public AchievementLevelEnum? AchievementLevelAwarded(int playerId, IDataContext dataContext)
        {
            var differentPlayedGamesCount =
                dataContext.GetQueryable<PlayerGameResult>()
                    .Where(pgr => pgr.PlayerId == playerId)
                    .Select(pgr => pgr.PlayedGame.GameDefinition.Id)
                    .Distinct()
                    .Count();
             

            return LevelThresholds.OrderByDescending(l => l.Value).FirstOrDefault(l => l.Value <= differentPlayedGamesCount).Key;
        }
    }
}