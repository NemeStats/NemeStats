using System.Collections.Generic;

namespace UI.Areas.Api.Models
{
    public class PlayersSearchResultsMessage
    {
        public List<PlayerSearchResultMessage> Players { get; set; } = new List<PlayerSearchResultMessage>();
    }
}
