using System.Collections.Generic;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Models.Points;

namespace BusinessLogic.Models.Players
{
    public class PlayerQuickStats
    {
        public int TotalGamesWon { get; set; }
        public NemePointsSummary NemePointsSummary { get; set; }
        public int TotalGamesPlayed { get; set; }
        public int? PlayerId { get; set; }
        public PlayedGameQuickStats LastGamingGroupGame { get; set; }
    }

    public class PlayersToCreateModel
    {
        public List<PlayerInfoForUser> RecentPlayers { get; set; }
        public List<PlayerInfoForUser> MostActivePlayers { get; set; }
        public List<PlayerInfoForUser> OtherPlayers { get; set; }
        public PlayerInfoForUser UserPlayer { get; set; }
    }
}
