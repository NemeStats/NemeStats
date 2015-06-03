using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI.Models.PlayedGame
{
    public class PlayedGamesFilterViewModel
    {
        public DateTime? DatePlayedStart { get; set; }
        public DateTime? DatePlayedEnd { get; set; }
        public int? GameDefinitionId { get; set; }
    }
}
