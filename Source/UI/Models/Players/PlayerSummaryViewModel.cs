using System.Collections.Generic;
using UI.Models.Badges;

namespace UI.Models.Players
{
    public class PlayerSummaryViewModel
    {
        public PlayerSummaryViewModel()
        {
            SpecialBadgeTypes = new List<IBadgeBaseViewModel>();
        }

        public string PlayerName { get; set; }
        public int PlayerId { get; set; }
        public int GamesWon { get; set; }
        public int GamesLost { get; set; }
        public int WinPercentage { get; set; }
        public int LostPercentage
        {
            get { return 100 - WinPercentage; }
        }

        public IList<IBadgeBaseViewModel> SpecialBadgeTypes { get; set; }
    }
}