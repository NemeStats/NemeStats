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
        public float WinPercentage { get; set; }
        public int NumberOfGames { get; set; }
        public int NumberOfWins { get; set; }
    }
}