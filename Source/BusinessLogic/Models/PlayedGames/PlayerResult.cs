using System;

namespace BusinessLogic.Models.PlayedGames
{
    public class PlayerResult
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int GameRank { get; set; }
        public int NemeStatsPointsAwarded { get; set; }
        public int? PointsScored { get; set; }
    }
}
