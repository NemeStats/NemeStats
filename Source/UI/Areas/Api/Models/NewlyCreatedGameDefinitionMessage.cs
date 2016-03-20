using System.Linq;

namespace UI.Areas.Api.Models
{
    public class NewlyCreatedGameDefinitionMessage
    {
        public int GameDefinitionId { get; set; }
        public int GamingGroupId { get; set; }
    }
}
