using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class SocialButterflyAchievement : IAchievement
    {
        public AchievementId Id => AchievementId.SocialButterfly;
        public AchievementGroup Group => AchievementGroup.Game;
        public string Name => "Social Butterfly";
        public string Description => "Play games with lots of different players";
        public string IconClass => "fa fa-smile-o";

        public Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 10},
            {AchievementLevel.Silver, 30},
            {AchievementLevel.Gold, 50}
        };

        public AchievementAwarded IsAwardedForThisPlayer(int playerId, IDataContext dataContext)
        {
            var result = new AchievementAwarded
            {
                AchievementId = Id
            };

            var totalPlayersPlayedWith =
                dataContext
                    .GetQueryable<PlayedGame>()
                    .Where(x => x.PlayerGameResults.Any(y => y.PlayerId == playerId))
                    .Select(z => z.PlayerGameResults.Select(x => x.PlayerId))
                    .Distinct()
                    .Count();

            if (totalPlayersPlayedWith < LevelThresholds[AchievementLevel.Bronze])
            {
                return result;
            }

            result.LevelAwarded =
                LevelThresholds.OrderByDescending(l => l.Value)
                    .FirstOrDefault(l => l.Value <= totalPlayersPlayedWith)
                    .Key;
            return result;
        }
    }
}
