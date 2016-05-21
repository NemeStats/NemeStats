using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class RiskTakerAchievement : BaseAchievement
    {
        public RiskTakerAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.RiskTaker;

        public override AchievementGroup Group => AchievementGroup.Game;

        public override string Name => "Risk Taker";

        public override string DescriptionFormat => "This achievement is earned by being the sole winner of games with 10+ people {0} times.";

        public override string IconClass => "ns-icon-one";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 1},
            {AchievementLevel.Silver, 4},
            {AchievementLevel.Gold, 20}
        };

        public override AchievementAwarded IsAwardedForThisPlayer(int playerId )
        {
            var result = new AchievementAwarded
            {
                AchievementId = Id
            };

            var numberOfGamesAsSoleWinner =
                DataContext
                    .GetQueryable<PlayedGame>()
                    .Count(x => x.PlayerGameResults.Any(y => y.PlayerId == playerId && y.GameRank == 1)
                        && !x.PlayerGameResults.Any(y => y.PlayerId != playerId && y.GameRank == 1)
                        && x.NumberOfPlayers >= 10);

            result.PlayerProgress = numberOfGamesAsSoleWinner;

            if (numberOfGamesAsSoleWinner < LevelThresholds[AchievementLevel.Bronze])
            {
                return result;
            }

            result.LevelAwarded = GetLevelAwarded(numberOfGamesAsSoleWinner);
            return result;
        }
    }
}
