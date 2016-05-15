using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{


    public class BusyBeeAchievement : BaseAchievement
    {
        public BusyBeeAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.BusyBee;

        public override AchievementGroup Group => AchievementGroup.Game;

        public override string Name => "Busy Bee";

        public override string Description => "Play lots of games";

        public override string IconClass => "fa fa-forumbee";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 20},
            {AchievementLevel.Silver, 80},
            {AchievementLevel.Gold, 300}
        };

        public override AchievementAwarded IsAwardedForThisPlayer(int playerId)
        {
            var result = new AchievementAwarded
            {
                AchievementId = Id
            };

            var totalGamesPlayed =
                DataContext
                    .GetQueryable<PlayerGameResult>()
                    .Count(pgr => pgr.PlayerId == playerId);

            if (totalGamesPlayed < LevelThresholds[AchievementLevel.Bronze])
            {
                return result;
            }

            result.LevelAwarded = GetLevelAwarded(totalGamesPlayed);
            return result;
        }
    }
}
