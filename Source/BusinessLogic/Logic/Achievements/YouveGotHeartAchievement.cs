using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class YouveGotHeartAchievement : BaseAchievement
    {
        public YouveGotHeartAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.YouveGotHeart;

        public override AchievementGroup Group => AchievementGroup.PlayedGame;

        public override string Name => "You've got heart";

        public override string DescriptionFormat => "Earn this Achievement by playing {0} games without winning even once.";

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
                    .Where(y => y.PlayerId == playerId)
                    .Select(pg => pg.PlayedGameId);

            result.PlayerProgress = numberOfGamesWithoutWinning.Count();
            result.RelatedEntities = numberOfGamesWithoutWinning.ToList();

            if (result.PlayerProgress < LevelThresholds[AchievementLevel.Bronze])
            {
                return result;
            }

            result.LevelAwarded = GetLevelAwarded(result.PlayerProgress);
            return result;
        }
    }
}
