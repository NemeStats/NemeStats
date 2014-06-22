using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Models.PlayedGame;

namespace UI.Models.Players
{
    public class PlayerDetailsViewModel
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public bool Active { get; set; }
        //public List<IndividualPlayerGameSummaryViewModel> PlayerGameSummaries { get; set; }
        public List<GameResultViewModel> PlayerGameResultDetails { get; set; }
        public int TotalGamesPlayed { get; set; }
    }
}
