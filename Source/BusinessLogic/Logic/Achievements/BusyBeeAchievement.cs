using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class BusyBeeAchievement : IAchievement
    {
        public AchievementId Id => AchievementId.BusyBee;
        public AchievementGroup Group => AchievementGroup.Game;
        public string Name => "Busy Bee";
        public string Description => "Play lots of games";
        public string IconClass => "fa fa-forumbee";

        public Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 20},
            {AchievementLevel.Silver, 80},
            {AchievementLevel.Gold, 300}
        };

        public AchievementAwarded IsAwardedForThisPlayer(int playerId, IDataContext dataContext)
        {
            var result = new AchievementAwarded
            {
                AchievementId = Id
            };

            var totalGamesPlayed =
                dataContext
                    .GetQueryable<PlayerGameResult>()
                    .Count(pgr => pgr.PlayerId == playerId);

            result.LevelAwarded =
                LevelThresholds.OrderByDescending(l => l.Value)
                    .FirstOrDefault(l => l.Value <= totalGamesPlayed)
                    .Key;
            return result;
        }
    }
}
