using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Models.PlayedGame
{
    public class PlayedGamesViewModel
    {
        public IList<PlayedGameDetailsViewModel> PlayedGameDetailsViewModels { get; set; }
        public string PanelTitle { get; set; }
    }
}