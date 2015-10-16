using System.Linq;

namespace BusinessLogic.Models.Players
{
    public class PlayerWinRecord
    {
        public string PlayerName { get; set; }
        public int PlayerId { get; set; }
        public int GamesWon { get; set; }
        public int GamesLost { get; set; }
        public int WinPercentage { get; set; }
        public bool IsChampion { get; set; }
        public bool IsFormerChampion { get; set; }
    }
}
