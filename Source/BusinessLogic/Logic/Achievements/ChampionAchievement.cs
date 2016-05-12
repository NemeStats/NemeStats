using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class ChampionAchievement : IAchievement
    {
        public AchievementId Id => AchievementId.Champion;
        public AchievementGroup Group => AchievementGroup.Game;

        public Dictionary<AchievementLevelEnum, int> LevelThresholds => new Dictionary<AchievementLevelEnum, int>
        {
            {AchievementLevelEnum.Bronze, 1},
            {AchievementLevelEnum.Silver, 10},
            {AchievementLevelEnum.Gold, 50}
        };

        public AchievementAwarded IsAwardedForThisPlayer(int playerId, IDataContext dataContext)
        {
            var result = new AchievementAwarded
            {
                AchievementId = this.Id
            };

            var championedGames = dataContext.GetQueryable<Champion>().Where(c=>c.PlayerId == playerId).Select(c=>c.GameDefinitionId);

            result.LevelAwarded =  LevelThresholds.OrderByDescending(l => l.Value).FirstOrDefault(l => l.Value <= championedGames.Count()).Key;
            result.RelatedEntities = championedGames.ToList();

            return result;
        }
    }
}