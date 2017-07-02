using System;
using System.Linq;

namespace UI.Models.PlayedGame
{
    public class PlayedGamesFilterViewModel
    {
        public DateTime? DatePlayedStart { get; set; }
        public DateTime? DatePlayedEnd { get; set; }
        public int? GameDefinitionId { get; set; }
        public int? IncludedPlayerId { get; set; }
    }
}
