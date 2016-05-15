using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class YouveGotHeartAchievement : IAchievement
    {
        public AchievementId Id => AchievementId.YouveGotHeart;
        public AchievementGroup Group => AchievementGroup.Game;
        public string Name => "You've got heart";
        public string Description => "Played a lot of games without winning";
        public string IconClass => "fa fa-heart";

        public Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 10},
            {AchievementLevel.Silver, 20},
            {AchievementLevel.Gold, 30}
        };

        public AchievementAwarded IsAwardedForThisPlayer(int playerId, IDataContext dataContext)
        {
            var result = new AchievementAwarded
            {
                AchievementId = Id
            };

            var playerHasAWin = dataContext.GetQueryable<PlayerGameResult>().Any(x => x.PlayerId == 1 && x.GameRank == 1);

            if (playerHasAWin)
            {
                return result;
            }

            //TODO how to combine this into a single query with the previous query?
            var numberOfGamesWithoutWinning =
                dataContext
                    .GetQueryable<PlayerGameResult>()
                    .Count(y => y.PlayerId == playerId);

            if (numberOfGamesWithoutWinning < LevelThresholds[AchievementLevel.Bronze])
            {
                return result;
            }

            result.LevelAwarded =
                LevelThresholds.OrderByDescending(l => l.Value)
                    .FirstOrDefault(l => l.Value <= numberOfGamesWithoutWinning)
                    .Key;
            return result;
        }
    }
}
