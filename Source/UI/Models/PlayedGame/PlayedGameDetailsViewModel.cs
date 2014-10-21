using System;
using System.Collections.Generic;
using System.Linq;

namespace UI.Models.PlayedGame
{
    public class PlayedGameDetailsViewModel
    {
        public int PlayedGameId { get; set; }
        public string GameDefinitionName { get; set; }
        public int GameDefinitionId { get; set; }
        public DateTime DatePlayed { get; set; }
        public IList<GameResultViewModel> PlayerResults { get; set; }
        public bool UserCanEdit { get; set; }
    }
}