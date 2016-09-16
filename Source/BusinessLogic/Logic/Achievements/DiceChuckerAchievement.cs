using System.Collections.Generic;
using BusinessLogic.DataAccess;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class DiceChuckerAchievement : CategoryAchievement
    {
        public DiceChuckerAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.DiceChucker;

        public override AchievementGroup Group => AchievementGroup.Game;

        public override string Name => "Dice Chucker";

        public override string DescriptionFormat => "This Achievement is earned by playing {0} different games with the BoardGameGeek Category of 'Dice'.";

        public override string IconClass => "ns-icon-dice-chucker";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 10},
            {AchievementLevel.Silver, 20},
            {AchievementLevel.Gold, 30}
        };

        public override string CategoryName=> "Dice";
    }
}
