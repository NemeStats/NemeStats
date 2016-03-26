using UI.Models.PlayedGame;
using UI.Models.Points;

namespace UI.Models.Players
{
    public class PlayerQuickStatsViewModel
    {
        public int TotalGamesWon { get; set; }
        public NemePointsSummaryViewModel NemePointsSummary { get; set; }
        public int TotalGamesPlayed { get; set; }
        public PlayedGameQuickStatsViewModel LastGamingGroupGame { get; set; }
        public int? PlayerId { get; internal set; }
    }
}