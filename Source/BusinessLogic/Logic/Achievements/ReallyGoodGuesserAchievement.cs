using System.Collections.Generic;
using BusinessLogic.DataAccess;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class ReallyGoodGuesserAchievement : MechanicAchievement
    {
        public ReallyGoodGuesserAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.ReallyGoodGuesser;

        public override AchievementGroup Group => AchievementGroup.Game;

        public override string Name => "Really Good Guesser";

        public override string DescriptionFormat => "This Achievement is earned by playing {0} different games with the BoardGameGeek Mechanic of 'Auction/Bidding'.";

        public override string IconClass => "ns-icon-gavel";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 10},
            {AchievementLevel.Silver, 20},
            {AchievementLevel.Gold, 30}
        };

        public override string MechanicName => "Auction/Bidding";

    }
}
