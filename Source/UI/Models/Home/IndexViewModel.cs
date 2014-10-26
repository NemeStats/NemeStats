using BusinessLogic.Models.Games;
using System.Collections.Generic;
using System.Linq;
using UI.Models.Players;

namespace UI.Models.Home
{
    public class HomeIndexViewModel
    {
        public List<TopPlayerViewModel> TopPlayers { get; set; }
        public List<PublicGameSummary> RecentPublicGames { get; set; }
    }
}