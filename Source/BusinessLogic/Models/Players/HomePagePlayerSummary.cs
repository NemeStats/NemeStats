using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Models.Points;

namespace BusinessLogic.Models.Players
{
    public class HomePagePlayerSummary
    {
        public int TotalGamesWon { get; set; }
        public NemePointsSummary NemePointsSummary { get; set; }
        public int TotalGamesPlayed { get; set; }
        public int? PlayerId { get; set; }
        public PlayedGameQuickStats LastGamingGroupPlayedGame { get; set; }
    }
}