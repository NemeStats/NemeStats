using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class ChampionAchievement : BaseAchievement
    {
        public ChampionAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.Champion;

        public override AchievementGroup Group => AchievementGroup.Game;

        public override string Name => "Champion";

        public override string Description => "Be the champion of several games";

        public override string IconClass => "fa fa-trophy";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 1},
            {AchievementLevel.Silver, 10},
            {AchievementLevel.Gold, 50}
        };

        public override AchievementAwarded IsAwardedForThisPlayer(int playerId)
        {
            var result = new AchievementAwarded
            {
                AchievementId = this.Id
            };

            var championedGames = DataContext.GetQueryable<Champion>().Where(c=>c.PlayerId == playerId).Select(c=>c.GameDefinitionId).Distinct();

            if (championedGames.Any())
            {
                result.LevelAwarded = GetLevelAwarded(championedGames.Count());
                result.RelatedEntities = championedGames.ToList();
            }
            
            return result;
        }
    }
}