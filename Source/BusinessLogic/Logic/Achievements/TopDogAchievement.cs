using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.Points;

namespace BusinessLogic.Logic.Achievements
{
    public class TopDogAchievement : BaseAchievement
    {
        public TopDogAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public static int MinNemePointsToUnlock => 250;
        public override AchievementId Id => AchievementId.TopDog;
        public override AchievementGroup Group => AchievementGroup.NotApply;
        public override string Name => "Top Dog";
        public override string DescriptionFormat => "Be the player with the most NemePoints in your gaming group. You need, at least, 250 NemePoints to unlock this achievement.";
        public override string IconClass => "ns-icon-medal";
        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Silver, 1}
        };
        public override AchievementAwarded IsAwardedForThisPlayer(int playerId)
        {
            var player = DataContext.GetQueryable<Player>().FirstOrDefault(p => p.Id == playerId);

            var result = new AchievementAwarded
            {
                AchievementId = Id
            };

            if (player != null)
            {

                var nemePointsDictionary = (from playerGameResult in DataContext.GetQueryable<PlayerGameResult>()
                                            where playerGameResult.PlayedGame.GamingGroupId == player.GamingGroupId
                                            group playerGameResult by playerGameResult.PlayerId
                    into groupedResults
                                            select
                                                new
                                                {
                                                    BasePoints = groupedResults.Sum(x => x.NemeStatsPointsAwarded),
                                                    GameDurationBonusPoints = groupedResults.Sum(x => x.GameDurationBonusPoints),
                                                    WeightBonusPoints = groupedResults.Sum(x => x.GameWeightBonusPoints),
                                                    PlayerId = groupedResults.Key
                                                }).ToDictionary(key => key.PlayerId,
                            value =>
                                new NemePointsSummary(value.BasePoints, value.GameDurationBonusPoints,
                                    value.WeightBonusPoints))
                    .OrderByDescending(r => r.Value.TotalPoints);

                if (nemePointsDictionary.First().Key == player.Id && nemePointsDictionary.First().Value.TotalPoints >= MinNemePointsToUnlock)
                {
                    result.LevelAwarded = LevelThresholds.First().Key;
                    result.PlayerProgress = 1;

                }
            }

            return result;
        }
    }
}