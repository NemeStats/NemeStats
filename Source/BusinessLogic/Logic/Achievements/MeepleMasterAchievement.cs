using System.Collections.Generic;
using BusinessLogic.DataAccess;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class MeepleMasterAchievement : MechanicAchievement
    {
        public MeepleMasterAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.MeepleMaster;

        public override AchievementGroup Group => AchievementGroup.Game;

        public override string Name => "Meeple Master";

        public override string DescriptionFormat => "This Achievement is earned by playing {0} different games with the BoardGameGeek Mechanic of 'Worker Placement'.";

        public override string IconClass => "ns-icon-meeple";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 10},
            {AchievementLevel.Silver, 20},
            {AchievementLevel.Gold, 30}
        };

        public override string MechanicName => "Worker Placement";

    }
}
