using System.Linq;

namespace UI.Models.Players
{
    public class PlayerGameSummaryViewModel
    {
        public string GameDefinitionName { get; set; }
        public int GameDefinitionId { get; set; }
        public int NumberOfGamesWon { get; set; }
        public int NumberOfGamesLost { get; set; }
        public int WinPercentage { get; set; }
        public bool IsChampion { get; set; }
        public bool IsFormerChampion { get; set; }
        public string ThumbnailImageUrl { get; set; }
    }
}
