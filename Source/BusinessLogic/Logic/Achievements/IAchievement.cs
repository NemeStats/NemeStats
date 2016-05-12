using System.Collections.Generic;
using BusinessLogic.DataAccess;
using BusinessLogic.Models;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public interface IAchievement
    {
        AchievementId Id { get; }
        AchievementGroup Group { get; }

        Dictionary<AchievementLevelEnum, int> LevelThresholds { get; }

        AchievementAwarded IsAwardedForThisPlayer(int playerId, IDataContext dataContext);
    }
}
