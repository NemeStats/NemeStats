using BusinessLogic.Models.Achievements;
using PagedList;

namespace BusinessLogic.Models.GamingGroups
{
    public class RecentGamingGroupChanges
    {
        public IPagedList<PlayerAchievementWinner> RecentAchievements { get; set; }
    }
}
