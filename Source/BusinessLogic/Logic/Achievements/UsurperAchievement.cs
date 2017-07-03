using System.Collections.Generic;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using System.Linq;

namespace BusinessLogic.Logic.Achievements
{
    public class UsurperAchievement : BaseAchievement
    {
        public UsurperAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.Usurper;

        public override AchievementGroup Group => AchievementGroup.Game;

        public override string Name => "Usurper";

        public override string DescriptionFormat => "This Achievement is earned by becoming the Champion of {0} different games where someone else was the Champion first.";

        public override string IconClass => "fa fa-hand-o-down";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 5},
            {AchievementLevel.Silver, 20},
            {AchievementLevel.Gold, 50}
        };

        public override AchievementAwarded IsAwardedForThisPlayer(int playerId)
        {
            var result = new AchievementAwarded
            {
                AchievementId = Id
            };

            var gamesPlayed =
                DataContext
                    .GetQueryable<Champion>()
                    .Where(champ => champ.PlayerId == playerId && champ.GameDefinition.PreviousChampion.PlayerId != playerId)
                    .GroupBy(x => new { x.PlayerId, x.GameDefinitionId })
                    .Select(group => group.Key);

            result.RelatedEntities = gamesPlayed.Select(x => x.GameDefinitionId).ToList();
            result.PlayerProgress = gamesPlayed.Count();

            if (result.PlayerProgress < LevelThresholds[AchievementLevel.Bronze])
            {
                return result;
            }

            result.LevelAwarded = GetLevelAwarded(result.PlayerProgress);
            return result;
        }
    }
}
