using System.Linq;

namespace UI.Models.Players
{
    public class PlayerWithNemesisViewModel
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int? NemesisPlayerId { get; set; }
        public string NemesisPlayerName { get; set; }
    }
}
