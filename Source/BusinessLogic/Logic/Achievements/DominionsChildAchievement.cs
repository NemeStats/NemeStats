using System.Collections.Generic;
using BusinessLogic.DataAccess;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class DominionsChildAchievement : MechanicAchievement
    {
        public DominionsChildAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.DominionsChild;

        public override AchievementGroup Group => AchievementGroup.Game;

        public override string Name => "Dominion's Child";

        public override string DescriptionFormat => "This Achievement is earned by playing {0} different games with the BoardGameGeek Mechanic of 'Deck / Pool Building'.";

        public override string IconClass => "ns-icon-mapbuilder";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 10},
            {AchievementLevel.Silver, 20},
            {AchievementLevel.Gold, 30}
        };

        public override string MechanicName => "Deck / Pool Building";

    }
}
