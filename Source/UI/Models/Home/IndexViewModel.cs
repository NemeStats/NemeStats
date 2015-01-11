using BusinessLogic.Models.Games;
using System.Collections.Generic;
using System.Linq;
using UI.Models.GamingGroup;
using UI.Models.Nemeses;
using UI.Models.Players;

namespace UI.Models.Home
{
    public class HomeIndexViewModel
    {
        public HomeIndexViewModel()
        {
            TopPlayers = new List<TopPlayerViewModel>();
            RecentPublicGames = new List<PublicGameSummary>();
            RecentNemesisChanges = new List<NemesisChangeViewModel>();
        }

        public List<TopPlayerViewModel> TopPlayers { get; set; }
        public List<PublicGameSummary> RecentPublicGames { get; set; }
        public List<NemesisChangeViewModel> RecentNemesisChanges { get; set; }
        public List<TopGamingGroupSummaryViewModel> TopGamingGroups { get; set; }
    }
}