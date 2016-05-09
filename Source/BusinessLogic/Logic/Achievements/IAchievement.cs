using System.Collections.Generic;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public interface IAchievement
    {
        AchievementType AchievementId { get; }

        Dictionary<AchievementLevelEnum, int> LevelThresholds { get; }

        AchievementLevelEnum? AchievementLevelAwarded(int playerId, IDataContext dataContext);
    }

}
