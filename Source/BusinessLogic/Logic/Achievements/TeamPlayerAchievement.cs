using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.PlayedGames;

namespace BusinessLogic.Logic.Achievements
{
    public class TeamPlayerAchievement : BaseAchievement
    {
        public TeamPlayerAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.TeamPlayer;

        public override AchievementGroup Group => AchievementGroup.PlayedGame;

        public override string Name => "Team Player";

        public override string DescriptionFormat => "This Achievement is earned by playing {0} games where everyone either won or lost.";

        public override string IconClass => "fa fa-users";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 10},
            {AchievementLevel.Silver, 30},
            {AchievementLevel.Gold, 50}
        };

        public override AchievementAwarded IsAwardedForThisPlayer(int playerId)
        {
            var result = new AchievementAwarded
            {
                AchievementId = Id
            };

            var allTeamGameIds =
                DataContext
                    .GetQueryable<PlayedGame>()
                    .Where(x => (x.WinnerType == WinnerTypes.TeamLoss || x.WinnerType == WinnerTypes.TeamWin)
                                && x.PlayerGameResults.Any(y => y.PlayerId == playerId))
                    .Select(z => z.Id)
                    .Distinct()
                    .ToList();

            result.PlayerProgress = allTeamGameIds.Count;
            result.RelatedEntities = allTeamGameIds;

            if (result.PlayerProgress < LevelThresholds[AchievementLevel.Bronze])
            {
                return result;
            }

            result.LevelAwarded = GetLevelAwarded(result.PlayerProgress);
               
            return result;
        }
    }
}
