using BusinessLogic.Models;
using UI.Models.Achievements;

namespace UI.Transformations
{
    public interface IPlayerAchievementViewModelBuilder
    {
        PlayerAchievementViewModel Build(PlayerAchievement playerAchievement);
    }
}