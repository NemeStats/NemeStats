using System.Collections.Generic;
using BusinessLogic.DataAccess;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class CivBuilderAchievement : CategoryAchievement
    {
        public CivBuilderAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.CivBuilder;

        public override AchievementGroup Group => AchievementGroup.Game;

        public override string Name => "Civ Builder";

        public override string DescriptionFormat => "This Achievement is earned by playing {0} different games with the BoardGameGeek Category of 'Civilization'.";

        public override string IconClass => "ns-icon-civbuilder";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 10},
            {AchievementLevel.Silver, 20},
            {AchievementLevel.Gold, 30}
        };

        public override string CategoryName => "Civilization";

    }
}
