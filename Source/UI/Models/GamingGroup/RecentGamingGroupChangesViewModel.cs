using System.Collections.Generic;
using PagedList;
using UI.Models.Nemeses;
using UI.Models.Players;

namespace UI.Models.GamingGroup
{
    public class RecentGamingGroupChangesViewModel
    {
        public IPagedList<PlayerAchievementWinnerViewModel> RecentAchievements { get; set; }
        public IList<NemesisChangeViewModel> RecentNemesisChanges { get; set; }
    }
}