using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class TotallyCategorizedAchievement : BaseAchievement
    {
        public TotallyCategorizedAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.TotallyCategorized;

        public override AchievementGroup Group => AchievementGroup.NotApplicable;

        public override string Name => "Totally Categorized";

        public override string DescriptionFormat => "This Achievement is earned by playing games with {0} unique BoardGameGeek game Categories. There are 84 unique categories as of Sept. 2016.";

        public override string IconClass => "ns-icon-categorized";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 25},
            {AchievementLevel.Silver, 60},
            {AchievementLevel.Gold, 84}
        };

        public override AchievementAwarded IsAwardedForThisPlayer(int playerId)
        {
            var result = new AchievementAwarded
            {
                AchievementId = Id
            };

            var allPlayedGamesWithCategories =
                DataContext
                    .GetQueryable<PlayerGameResult>()
                    .Where(x => x.PlayerId == playerId)
                    .Select(x => x.PlayedGame.GameDefinition.BoardGameGeekGameDefinition.Categories)
                    .Where(y => y.Count != 0)
                    .ToList();

            // Brute forced, could not figure out LINQ query for this. Probably needs cleaning up.
            List<string> finalCategoryList = new List<string>();
            foreach (var subList in allPlayedGamesWithCategories)
            {
                foreach (var subSubList in subList)
                {
                    finalCategoryList.Add(subSubList.CategoryName);
                }
            }

            result.PlayerProgress = finalCategoryList.Distinct().Count();
            //result.RelatedEntities = allPlayedCategories.ToList();

            if (result.PlayerProgress < LevelThresholds[AchievementLevel.Bronze])
            {
                return result;
            }

            result.LevelAwarded = GetLevelAwarded(result.PlayerProgress);
            return result;
        }
    }
}
