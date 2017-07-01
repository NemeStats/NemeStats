using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace UI.Models.PlayedGame
{
    public class SearchViewModel
    {
        public SearchViewModel()
        {
            Filter = new PlayedGamesFilterViewModel();
        }

        public IList<SelectListItem> GameDefinitions { get; set; }
        public PlayedGamesFilterViewModel Filter { get; set; }
        public PlayedGamesViewModel PlayedGames { get; set; }
        public IEnumerable<SelectListItem> Players { get; set; }
    }
}