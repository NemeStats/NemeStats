using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class SoCloseAchievement : BaseAchievement
    {
        public SoCloseAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.SoClose;
        public override AchievementGroup Group => AchievementGroup.PlayedGame;
        public override string Name => "So Close!";
        public override string DescriptionFormat => "This Achievement is earned by placing second, and losing by only 1 point, in {0} games.";
        public override string IconClass => "ns-icon-second-place";

        private class GameScores
        {
            public int PlayedGameId { get; set; }
            public int GameRank { get; set; }
            public decimal? PointsScored { get; set; }
        };

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 10},
            {AchievementLevel.Silver, 25},
            {AchievementLevel.Gold, 50}
        };

        public override AchievementAwarded IsAwardedForThisPlayer(int playerId)
        {
            var result = new AchievementAwarded
            {
                AchievementId = Id
            };

            var allPlayedGameIdsWherePlayerRankedSecond =
                DataContext
                    .GetQueryable<PlayerGameResult>()
                    .Where(x => x.PlayerId == playerId && x.GameRank == 2 && x.PointsScored != null)
                    .Select(x => x.PlayedGameId)
                    .ToList();

            var gamesWherePlayerRankedSecondRankTwo =
                DataContext
                    .GetQueryable<PlayerGameResult>()
                    .Where(x => x.PlayerId == playerId && x.GameRank == 2 && x.PointsScored != null)
                    .Select(x => new GameScores() { PlayedGameId = x.PlayedGameId, GameRank = x.GameRank, PointsScored = x.PointsScored } )
                    .ToList();

            var gamesWherePlayerRankedSecondRankOne =
                DataContext
                    .GetQueryable<PlayerGameResult>()
                    .Where(x => allPlayedGameIdsWherePlayerRankedSecond.Contains(x.PlayedGameId) && x.GameRank == 1 && x.PointsScored != null )
                    .Select(x => new GameScores() { PlayedGameId = x.PlayedGameId, GameRank = x.GameRank, PointsScored = x.PointsScored })
                    .ToList();

            var combinedList =
                from rankOne in gamesWherePlayerRankedSecondRankOne
                join rankTwo in gamesWherePlayerRankedSecondRankTwo on rankOne.PlayedGameId equals rankTwo.PlayedGameId
                where (rankOne.PointsScored - rankTwo.PointsScored.Value) == 1
                select rankOne.PlayedGameId;

            result.PlayerProgress = combinedList.Count();
            result.RelatedEntities = combinedList.ToList();

            if (result.PlayerProgress < LevelThresholds[AchievementLevel.Bronze])
            {
                return result;
            }

            result.LevelAwarded = GetLevelAwarded(result.PlayerProgress);
            return result;
        }
    }
}
