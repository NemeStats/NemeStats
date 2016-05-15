using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class RiskTakerAchievement : IAchievement
    {
        public AchievementId Id => AchievementId.RiskTaker;
        public AchievementGroup Group => AchievementGroup.Game;
        public string Name => "Social Butterfly";
        public string Description => "Be the only player to get first place in games with 10+ people";
        public string IconClass => "fa fa-ambulance";

        public Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 1},
            {AchievementLevel.Silver, 4},
            {AchievementLevel.Gold, 20}
        };

        public AchievementAwarded IsAwardedForThisPlayer(int playerId, IDataContext dataContext)
        {
            var result = new AchievementAwarded
            {
                AchievementId = Id
            };

            var numberOfGamesAsSoleWinner =
                dataContext
                    .GetQueryable<PlayedGame>()
                    .Count(x => x.PlayerGameResults.Any(y => y.PlayerId == playerId && y.GameRank == 1)
                        && !x.PlayerGameResults.Any(y => y.PlayerId != playerId && y.GameRank == 1)
                        && x.NumberOfPlayers >= 10);

            if (numberOfGamesAsSoleWinner < LevelThresholds[AchievementLevel.Bronze])
            {
                return result;
            }

            result.LevelAwarded =
                LevelThresholds.OrderByDescending(l => l.Value)
                    .FirstOrDefault(l => l.Value <= numberOfGamesAsSoleWinner)
                    .Key;
            return result;
        }
    }
}
