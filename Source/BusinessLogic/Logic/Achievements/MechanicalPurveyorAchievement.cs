using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class MechanicalPurveyorAchievement : BaseAchievement
    {
        public MechanicalPurveyorAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.MechanicalPurveyor;

        public override AchievementGroup Group => AchievementGroup.NotApplicable;

        public override string Name => "Mechanical Purveyor";

        public override string DescriptionFormat => "This Achievement is earned by playing games with {0} unique BoardGameGeek game Mechanics. There are 51 unique mechanics as of Sept. 2016.";

        public override string IconClass => "ns-icon-mechanical";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 15},
            {AchievementLevel.Silver, 30},
            {AchievementLevel.Gold, 51}
        };

        public override AchievementAwarded IsAwardedForThisPlayer(int playerId)
        {
            var result = new AchievementAwarded
            {
                AchievementId = Id
            };

            var allPlayedGamesWithMechanics =
                DataContext
                    .GetQueryable<PlayerGameResult>()
                    .Where(x => x.PlayerId == playerId)
                    .Select(x => x.PlayedGame.GameDefinition.BoardGameGeekGameDefinition.Mechanics)
                    .Where(y => y.Count != 0)
                    .ToList();

            // Brute forced, could not figure out LINQ query for this. Probably needs cleaning up.
            List<string> finalMechanicList = new List<string>();
            foreach (var subList in allPlayedGamesWithMechanics)
            {
                foreach (var subSubList in subList)
                {
                    finalMechanicList.Add(subSubList.MechanicName);
                }
            }

            result.PlayerProgress = finalMechanicList.Distinct().Count();
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
