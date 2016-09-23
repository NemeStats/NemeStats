using System.Collections.Generic;
using BusinessLogic.DataAccess;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class MapBuilderAchievement : MechanicAchievement
    {
        public MapBuilderAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.MapBuilder;

        public override AchievementGroup Group => AchievementGroup.Game;

        public override string Name => "Map Builder";

        public override string DescriptionFormat => "This Achievement is earned by playing {0} different games with the BoardGameGeek Mechanic of 'Tile Placement'.";

        public override string IconClass => "ns-icon-mapbuilder";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 10},
            {AchievementLevel.Silver, 20},
            {AchievementLevel.Gold, 30}
        };

        public override string MechanicName => "Tile Placement";

    }
}
