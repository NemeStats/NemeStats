using System.Collections.Generic;
using BusinessLogic.DataAccess;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public interface IAchievement
    {
        IDataContext DataContext { get; set; }
        AchievementId Id { get; }
        AchievementGroup Group { get; }
        string Name { get; }
        string DescriptionFormat { get; }
        string Description { get; }
        string IconClass { get; }

        Dictionary<AchievementLevel, int> LevelThresholds { get; }
        AchievementAwarded IsAwardedForThisPlayer(int playerId);
    }
}
