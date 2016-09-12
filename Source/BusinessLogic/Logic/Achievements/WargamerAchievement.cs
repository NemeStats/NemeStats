using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class WargamerAchievement : BaseAchievement
    {
        public WargamerAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.Wargamer;

        public override AchievementGroup Group => AchievementGroup.Game;

        public override string Name => "Wargamer!";

        public override string DescriptionFormat => "This Achievement is earned by playing {0} different games with the BoardGameGeek Category of 'Wargame'.";

        public override string IconClass => "ns-icon-wargamer";

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

            var allDiceGames =
                DataContext
                    .GetQueryable<PlayerGameResult>()
                    .Where(x => x.PlayerId == playerId
                        && x.PlayedGame.GameDefinition.BoardGameGeekGameDefinition.Categories.Any(o => o.CategoryName == "Wargame"))
                    .Select(x => x.PlayedGame.GameDefinitionId)
                    .Distinct()
                    .ToList();

            result.PlayerProgress = allDiceGames.Count;
            result.RelatedEntities = allDiceGames.ToList();

            if (result.PlayerProgress < LevelThresholds[AchievementLevel.Bronze])
            {
                return result;
            }

            result.LevelAwarded = GetLevelAwarded(result.PlayerProgress);
            return result;
        }
    }
}
