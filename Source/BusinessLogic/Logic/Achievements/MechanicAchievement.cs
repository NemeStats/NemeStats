using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public abstract class MechanicAchievement : BaseAchievement
    {
        protected MechanicAchievement(IDataContext dataContext) : base(dataContext)
        {
        }

        public abstract string MechanicName { get; }

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
                                && x.PlayedGame.GameDefinition.BoardGameGeekGameDefinition.Mechanics.Any(o => o.MechanicName == MechanicName))
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