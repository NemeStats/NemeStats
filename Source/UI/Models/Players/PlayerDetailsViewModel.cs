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
        public List<GameResultViewModel> PlayerGameResultDetails { get; set; }
        public int TotalGamesPlayed { get; set; }
        public int TotalPoints { get; set; }
        public float AveragePointsPerGame { get; set; }
        public float AveragePlayersPerGame { get; set; }
        public float AveragePointsPerPlayer { get; set; }
        public bool HasNemesis { get; set; }
        public int NemesisPlayerId { get; set; }
        public string NemesisName { get; set; }
        public float LossPercentageVersusPlayer { get; set; }
        public int NumberOfGamesLostVersusNemesis { get; set; }
        public bool UserCanEdit { get; set; }
        public IList<MinionViewModel> Minions { get; set; }
    }
}
