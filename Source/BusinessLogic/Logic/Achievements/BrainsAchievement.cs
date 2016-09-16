using System.Collections.Generic;
using BusinessLogic.DataAccess;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class BrainsAchievement : CategoryAchievement
    {
        public BrainsAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.Brains;

        public override AchievementGroup Group => AchievementGroup.Game;

        public override string Name => "BRAINS!";

        public override string DescriptionFormat => "{0} BRAINNNNNNSSSS!";

        public override string IconClass => "ns-icon-brains";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 5},
            {AchievementLevel.Silver, 10},
            {AchievementLevel.Gold, 20}
        };

        public override string CategoryName => "Zombies";
    }
}
