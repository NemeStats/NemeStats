using System.Collections.Generic;

namespace BusinessLogic.Logic.Achievements
{
    public interface IAchievementAwarder
    {
        void AwardNewAchievements(List<int> playerIds);
    }
}