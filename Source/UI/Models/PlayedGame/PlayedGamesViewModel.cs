using System.Collections.Generic;
using System.Linq;

namespace UI.Models.PlayedGame
{
    public class PlayedGamesViewModel : IEditableViewModel
    {
        public IList<PlayedGameDetailsViewModel> PlayedGameDetailsViewModels { get; set; }
        public string PanelTitle { get; set; }
        public bool UserCanEdit { get; set; }
        public int? GamingGroupId { get; set; }
        public bool ShowSearchLinkInResultsHeader { get; set; }
    }
}