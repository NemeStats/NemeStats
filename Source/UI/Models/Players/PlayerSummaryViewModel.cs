using System.Collections.Generic;
using UI.Models.Badges;

namespace UI.Models.Players
{
    public class PlayerSummaryViewModel
    {

        public string PlayerName { get; set; }
        public int PlayerId { get; set; }
        public int GamesWon { get; set; }
        public int GamesLost { get; set; }
        public int WinPercentage { get; set; }
        public IList<IBadgeBaseViewModel> SpecialBadgeTypes { get; set; } = new List<IBadgeBaseViewModel>();
    }
}