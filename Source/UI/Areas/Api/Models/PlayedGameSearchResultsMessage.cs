using System.Collections.Generic;
using System.Linq;

namespace UI.Areas.Api.Models
{
    public class PlayedGameSearchResultsMessage
    {
        public List<PlayedGameSearchResultMessage> PlayedGames { get; set; }
    }
}
