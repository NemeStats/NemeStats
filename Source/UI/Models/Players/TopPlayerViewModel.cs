using System.Linq;

namespace UI.Models.Players
{
    public class TopPlayerViewModel
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int TotalNumberOfGamesPlayed { get; set; }
        public int WinPercentage { get; set; }
        public int TotalPoints { get; set; }
    }
}
