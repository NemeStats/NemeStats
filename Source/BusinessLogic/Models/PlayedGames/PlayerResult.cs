using System;

namespace BusinessLogic.Models.PlayedGames
{
    public class PlayerResult
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public bool PlayerActive { get; set; }
        public int GameRank { get; set; }
        public int NemeStatsPointsAwarded { get; set; }
        public int? PointsScored { get; set; }
        public int PlayedGameId { get; set; }
        public DateTime DatePlayed { get; set; }
        public string GameName { get; set; }
        public int GameDefinitionId { get; set; }
        public int GameWeightBonusNemePoints { get; set; }
        public int GameDurationBonusNemePoints { get; set; }
        public int TotalPoints { get; set; }
    }
}
