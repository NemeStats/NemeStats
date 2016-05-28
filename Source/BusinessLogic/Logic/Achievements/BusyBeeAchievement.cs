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

        public override AchievementGroup Group => AchievementGroup.PlayedGame;

        public override string Name => "Busy Bee";

        public override string DescriptionFormat => "This Achievement is earned by playing {0} Games -- it doesn't matter if it is a win or a loss.";

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

            var gamesPlayed =
                DataContext
                    .GetQueryable<PlayerGameResult>()
                    .Where(pgr => pgr.PlayerId == playerId)
                    .Select(pg=>pg.PlayedGameId);

            result.RelatedEntities = gamesPlayed.ToList();
            result.PlayerProgress = gamesPlayed.Count();

            if (result.PlayerProgress < LevelThresholds[AchievementLevel.Bronze])
            {
                return result;
            }

            result.LevelAwarded = GetLevelAwarded(result.PlayerProgress);
            return result;
        }
    }
}
