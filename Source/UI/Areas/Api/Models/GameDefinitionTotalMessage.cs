using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI.Areas.Api.Models
{
    public class GameDefinitionTotalMessage
    {
        public int GameDefinitionId { get; set; }
        public string GameDefinitionName { get; set; }
        public int GamesWon { get; set; }
        public int GamesPlayed { get; set; }
    }
}
