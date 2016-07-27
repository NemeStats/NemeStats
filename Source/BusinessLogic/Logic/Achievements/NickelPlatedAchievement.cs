using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class NickelPlatedAchievement : BaseAchievement
    {
        public NickelPlatedAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.NickelPlated;

        public override AchievementGroup Group => AchievementGroup.Game;

        public override string Name => "Nickel-Plated";

        public override string DescriptionFormat => "This Achievement is earned by playing {0} games at least 5 times each.";

        public override string IconClass => "fa fa-html5";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 5},
            {AchievementLevel.Silver, 10},
            {AchievementLevel.Gold, 25}
        };

        public override AchievementAwarded IsAwardedForThisPlayer(int playerId)
        {
            var result = new AchievementAwarded
            {
                AchievementId = Id
            };

            var allNickeledGameIds =
                DataContext
                    .GetQueryable<PlayerGameResult>()
                    .Where(x => x.PlayerId == playerId)
                    .GroupBy(x => x.PlayedGame.GameDefinitionId)
                    .Select(group => new { group.Key, Count = group.Count() })
                    .Where(a => a.Count >= 5)
                    .ToList();

            result.PlayerProgress = allNickeledGameIds.Count();
            result.RelatedEntities = allNickeledGameIds.Select(y => y.Key).ToList();

            if (result.PlayerProgress < LevelThresholds[AchievementLevel.Bronze])
            {
                return result;
            }

            result.LevelAwarded = GetLevelAwarded(result.PlayerProgress);

            return result;
        }
    }
}
