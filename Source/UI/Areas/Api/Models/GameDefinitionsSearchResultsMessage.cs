using System.Collections.Generic;
using System.Linq;

namespace UI.Areas.Api.Models
{
    public class GameDefinitionsSearchResultsMessage
    {
        public List<GameDefinitionSearchResultMessage> GameDefinitions { get; set; }
    }
}
