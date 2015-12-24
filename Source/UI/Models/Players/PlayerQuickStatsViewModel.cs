using UI.Models.PlayedGame;

namespace UI.Models.Players
{
    public class PlayerQuickStatsViewModel
    {
        public int TotalGamesWon { get; set; }
        public int TotalPoints { get; set; }
        public int TotalGamesPlayed { get; set; }
        public PlayedGameQuickStatsViewModel LastGamingGroupGame { get; set; }
        public int? PlayerId { get; internal set; }
    }
}