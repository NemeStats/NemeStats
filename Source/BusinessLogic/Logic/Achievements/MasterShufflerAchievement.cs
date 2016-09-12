using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class MasterShufflerAchievement : BaseAchievement
    {
        public MasterShufflerAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public override AchievementId Id => AchievementId.MasterShuffler;

        public override AchievementGroup Group => AchievementGroup.Game;

        public override string Name => "Master Shuffler";

        public override string DescriptionFormat => "This Achievement is earned by playing {0} different games with the BoardGameGeek Category of 'Card Game'.";

        public override string IconClass => "ns-icon-mastershuffler";

        public override Dictionary<AchievementLevel, int> LevelThresholds => new Dictionary<AchievementLevel, int>
        {
            {AchievementLevel.Bronze, 25},
            {AchievementLevel.Silver, 50},
            {AchievementLevel.Gold, 75}
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
                        && x.PlayedGame.GameDefinition.BoardGameGeekGameDefinition.Categories.Any(o => o.CategoryName == "Card Game"))
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
