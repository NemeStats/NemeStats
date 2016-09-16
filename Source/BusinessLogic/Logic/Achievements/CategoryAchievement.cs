using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public abstract class CategoryAchievement : BaseAchievement
    {
        protected CategoryAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public abstract string CategoryName { get; }

        public override AchievementAwarded IsAwardedForThisPlayer(int playerId)
        {
            var result = new AchievementAwarded
            {
                AchievementId = Id
            };

            var categoryGames =
                DataContext
                    .GetQueryable<PlayerGameResult>()
                    .Where(x => x.PlayerId == playerId
                                && x.PlayedGame.GameDefinition.BoardGameGeekGameDefinition.Categories.Any(o => o.CategoryName == CategoryName))
                    .Select(x => x.PlayedGame.GameDefinitionId)
                    .Distinct()
                    .ToList();

            result.PlayerProgress = categoryGames.Count;
            result.RelatedEntities = categoryGames.ToList();

            if (result.PlayerProgress < LevelThresholds[AchievementLevel.Bronze])
            {
                return result;
            }

            result.LevelAwarded = GetLevelAwarded(result.PlayerProgress);
            return result;
        }
    }
}