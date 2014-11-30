using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Models
{
    public class ChampionViewModel
    {
        public string GameDefinitionName { get; set; }
        public int GameDefinitionId { get; set; }
        public int WinPercentage { get; set; }
        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }
    }
}