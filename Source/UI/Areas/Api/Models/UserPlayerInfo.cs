using System.Linq;

namespace UI.Areas.Api.Models
{
    public class PlayerInfoForUserMessage
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int GamingGroupId { get; set; }
    }
}
