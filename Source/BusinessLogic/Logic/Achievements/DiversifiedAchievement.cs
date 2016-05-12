using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class DiversifiedAchievement : IAchievement
    {
        public AchievementId Id => AchievementId.Diversified;
        public AchievementGroup Group => AchievementGroup.Game;

        public Dictionary<AchievementLevelEnum, int> LevelThresholds => new Dictionary<AchievementLevelEnum, int>
        {
            {AchievementLevelEnum.Bronze, 5},
            {AchievementLevelEnum.Silver, 25},
            {AchievementLevelEnum.Gold, 100}
        };

        public AchievementAwarded IsAwardedForThisPlayer(int playerId, IDataContext dataContext)
        {

            var result = new AchievementAwarded
            {
                AchievementId = this.Id
            };

            var differentPlayedGames =
                dataContext.GetQueryable<PlayerGameResult>()
                    .Where(pgr => pgr.PlayerId == playerId)
                    .Select(pgr => pgr.PlayedGame.GameDefinition.Id)
                    .Distinct();

            result.LevelAwarded =
                LevelThresholds.OrderByDescending(l => l.Value)
                    .FirstOrDefault(l => l.Value <= differentPlayedGames.Count())
                    .Key;
            result.RelatedEntities = differentPlayedGames.ToList();
            return result;
        }
    }
}