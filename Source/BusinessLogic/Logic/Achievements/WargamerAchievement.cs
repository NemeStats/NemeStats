using System.Collections.Generic;
using BusinessLogic.DataAccess;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class WargamerAchievement : CategoryAchievement
    {
        public WargamerAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.Wargamer;

        public override AchievementGroup Group => AchievementGroup.Game;

        public override string Name => "Wargamer!";

        public override string DescriptionFormat => "This Achievement is earned by playing {0} different games with the BoardGameGeek Category of 'Wargame'.";

        public override string IconClass => "ns-icon-wargamer";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 10},
            {AchievementLevel.Silver, 20},
            {AchievementLevel.Gold, 30}
        };

        public override string CategoryName => "Wargame";
    }
}
