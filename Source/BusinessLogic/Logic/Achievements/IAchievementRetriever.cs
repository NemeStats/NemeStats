using System.Collections.Generic;
using BusinessLogic.Models.Achievements;
using BusinessLogic.Models.User;

namespace BusinessLogic.Logic.Achievements
{
    public interface IAchievementRetriever
    {
        List<AggregateAchievementSummary> GetAllAchievementSummaries(ApplicationUser currentUser);
        IAchievement GetAchievement(AchievementId achievementId);
    }
}
