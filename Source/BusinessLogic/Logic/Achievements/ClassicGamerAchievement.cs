using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class ClassicGamerAchievement : BaseAchievement
    {
        public ClassicGamerAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.ClassicGamer;

        public override AchievementGroup Group => AchievementGroup.Player;

        public override string Name => "Classic Gamer";

        public override string DescriptionFormat => "This Achievement is earned by playing games from {0} different years.";

        public override string IconClass => "fa fa-calendar";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 5},
            {AchievementLevel.Silver, 10},
            {AchievementLevel.Gold, 25}
        };

        public override AchievementAwarded IsAwardedForThisPlayer(int playerId)
        {
            var result = new AchievementAwarded
            {
                AchievementId = Id
            };

            var yearsOfGamesPlayed =
                DataContext
                    .GetQueryable<PlayerGameResult>()
                    .Where(x => x.PlayerId == playerId && x.PlayedGame.GameDefinition.BoardGameGeekGameDefinition.YearPublished != null)
                    .GroupBy(x => x.PlayedGame.GameDefinition.BoardGameGeekGameDefinition.YearPublished)
                    .Select(y => y.Key)
                    .ToList();

            result.PlayerProgress = yearsOfGamesPlayed.Count();

            if (result.PlayerProgress < LevelThresholds[AchievementLevel.Bronze])
            {
                return result;
            }

            result.LevelAwarded = GetLevelAwarded(result.PlayerProgress);
            return result;
        }
    }
}
