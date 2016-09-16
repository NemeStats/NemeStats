using System.Collections.Generic;
using BusinessLogic.DataAccess;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class MasterShufflerAchievement : CategoryAchievement
    {
        public MasterShufflerAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.MasterShuffler;

        public override AchievementGroup Group => AchievementGroup.Game;

        public override string Name => "Master Shuffler";

        public override string DescriptionFormat => "This Achievement is earned by playing {0} different games with the BoardGameGeek Category of 'Card Game'.";

        public override string IconClass => "ns-icon-mastershuffler";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 25},
            {AchievementLevel.Silver, 50},
            {AchievementLevel.Gold, 75}
        };

        public override string CategoryName => "Card Game";

    }
}
