using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class BrainsAchievement : BaseAchievement
    {
        public BrainsAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.Brains;

        public override AchievementGroup Group => AchievementGroup.Game;

        public override string Name => "BRAINNNNNNSSSS!";

        public override string DescriptionFormat => "{0} BRAINNNNNNSSSS!";

        public override string IconClass => "ns-icon-brains";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 5},
            {AchievementLevel.Silver, 10},
            {AchievementLevel.Gold, 20}
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
                        && x.PlayedGame.GameDefinition.BoardGameGeekGameDefinition.Categories.Any(o => o.CategoryName == "Zombies"))
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
