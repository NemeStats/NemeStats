using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.PlayedGames;

namespace BusinessLogic.Logic.Achievements
{
    public class UnitedWeFallAchievement : BaseAchievement
    {
        public UnitedWeFallAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.UnitedWeFall;
        public override AchievementGroup Group => AchievementGroup.PlayedGame;
        public override string Name => "United We Fall";

        public override string DescriptionFormat => @"This Achievement is earned by losing {0} team games (where each player has the same rank at the end of the game).";

        public override string IconClass => "fa fa-frown-o";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 5},
            {AchievementLevel.Silver, 25},
            {AchievementLevel.Gold, 100}
        };

        public override AchievementAwarded IsAwardedForThisPlayer(int playerId)
        {
            var result = new AchievementAwarded
            {
                AchievementId = Id
            };

            var totalTeamLosses =
                DataContext
                    .GetQueryable<PlayedGame>()
                    .Where(x => x.PlayerGameResults.Any(player => player.PlayerId == playerId)
                        && x.WinnerType == WinnerTypes.TeamLoss)
                    .Select(x => x.Id)
                    .ToList();

            result.PlayerProgress = totalTeamLosses.Count;
            result.RelatedEntities = totalTeamLosses.ToList();
            if (result.PlayerProgress < LevelThresholds[AchievementLevel.Bronze])
            {
                return result;
            }

            result.LevelAwarded =
                LevelThresholds.OrderByDescending(l => l.Value)
                    .FirstOrDefault(l => l.Value <= result.PlayerProgress)
                    .Key;
            return result;
        }
    }
}
