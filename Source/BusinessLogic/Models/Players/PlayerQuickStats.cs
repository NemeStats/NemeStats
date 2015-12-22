using BusinessLogic.Models.PlayedGames;
using System;

namespace BusinessLogic.Models.Players
{
    public class PlayerQuickStats
    {
        public int TotalGamesWon { get; set; }
        public int TotalPoints { get; set; }
        public int TotalGamesPlayed { get; set; }
        public int PlayerId { get; set; }
        public PlayedGameQuickStats LastGamingGroupGame { get; set; }
    }
}
