using System.Collections.Generic;

namespace UI.Models.Players
{
    public class PlayersSummaryViewModel
    {
        public PlayersSummaryViewModel()
        {
            PlayerSummaries = new List<PlayerSummaryViewModel>();
        }

        public List<PlayerSummaryViewModel> PlayerSummaries { get; set; }
        public string WinLossHeader { get; set; }
    }
}