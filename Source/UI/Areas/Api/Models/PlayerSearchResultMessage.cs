
namespace UI.Areas.Api.Models
{
    public class PlayerSearchResultMessage
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public bool Active { get; set; }
        public int? CurrentNemesisPlayerId { get; set; }
    }
}
