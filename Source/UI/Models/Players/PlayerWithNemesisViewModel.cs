using System.Linq;

namespace UI.Models.Players
{
    public class PlayerWithNemesisViewModel : IEditableViewModel
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public bool PlayerRegistered { get; set; }
        public int? NemesisPlayerId { get; set; }
        public string NemesisPlayerName { get; set; }
        public int? PreviousNemesisPlayerId { get; set; }
        public string PreviousNemesisPlayerName { get; set; }
        public bool UserCanEdit { get; set; }
    }
}
