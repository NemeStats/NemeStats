using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class SocialButterflyAchievement : BaseAchievement
    {
        public SocialButterflyAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.SocialButterfly;

        public override AchievementGroup Group => AchievementGroup.Game;

        public override string Name => "Social Butterfly";

        public override string Description => "This Achievement is earned by playing games with lots of different Players.";

        public override string IconClass => "fa fa-smile-o";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 10},
            {AchievementLevel.Silver, 30},
            {AchievementLevel.Gold, 50}
        };

        public override AchievementAwarded IsAwardedForThisPlayer(int playerId)
        {
            var result = new AchievementAwarded
            {
                AchievementId = Id
            };

            var totalPlayersPlayedWith =
                DataContext
                    .GetQueryable<PlayedGame>()
                    .Where(x => x.PlayerGameResults.Any(y => y.PlayerId == playerId))
                    .Select(z => z.PlayerGameResults.Select(x => x.PlayerId))
                    .Distinct()
                    .Count();

            result.PlayerProgress = totalPlayersPlayedWith;

            if (totalPlayersPlayedWith < LevelThresholds[AchievementLevel.Bronze])
            {
                return result;
            }

            result.LevelAwarded = GetLevelAwarded(totalPlayersPlayedWith);
               
            return result;
        }
    }
}
