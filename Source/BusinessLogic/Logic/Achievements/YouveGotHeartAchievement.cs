using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using RollbarSharp.Serialization;

namespace BusinessLogic.Logic.Achievements
{
    public class YouveGotHeartAchievement : BaseAchievement
    {
        public YouveGotHeartAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.YouveGotHeart;

        public override AchievementGroup Group => AchievementGroup.Game;

        public override string Name => "You've got heart";

        public override string DescriptionFormat => "Earn this achievement by playing {0} games without winning even once.";

        public override string IconClass => "fa fa-heart";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 10},
            {AchievementLevel.Silver, 20},
            {AchievementLevel.Gold, 30}
        };

        public override AchievementAwarded IsAwardedForThisPlayer(int playerId)
        {
            var result = new AchievementAwarded
            {
                AchievementId = Id
            };

            var playerHasAWin = DataContext.GetQueryable<PlayerGameResult>().Any(x => x.PlayerId == playerId && x.GameRank == 1);

            if (playerHasAWin)
            {
                return result;
            }

            //TODO how to combine this into a single query with the previous query?
            var numberOfGamesWithoutWinning =
                DataContext
                    .GetQueryable<PlayerGameResult>()
                    .Count(y => y.PlayerId == playerId);

            result.PlayerProgress = numberOfGamesWithoutWinning;

            if (numberOfGamesWithoutWinning < LevelThresholds[AchievementLevel.Bronze])
            {
                return result;
            }

            result.LevelAwarded = GetLevelAwarded(numberOfGamesWithoutWinning);
            return result;
        }
    }
}
