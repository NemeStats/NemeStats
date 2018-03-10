using PagedList;
using UI.Models.Players;

namespace UI.Models.GamingGroup
{
    public class RecentGamingGroupChangesViewModel
    {
        public IPagedList<PlayerAchievementWinnerViewModel> RecentAchievements { get; set; }
    }
}