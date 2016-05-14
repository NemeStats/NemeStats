using BusinessLogic.Logic.Achievements;
using UI.Models.Achievements;

namespace UI.Transformations
{
    public interface IAchievementViewModelBuilder
    {
        AchievementViewModel Build(IAchievement achievement);
    }

    public class AchievementViewModelBuilder : IAchievementViewModelBuilder
    {
        public AchievementViewModel Build(IAchievement achievement)
        {
            return new AchievementViewModel
            {
                LevelThresholds = achievement.LevelThresholds,
                Name = achievement.Name,
                Description = achievement.Description,
                IconClass = achievement.IconClass
            };

        }
    }
}