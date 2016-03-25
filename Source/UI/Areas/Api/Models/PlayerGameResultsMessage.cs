namespace UI.Areas.Api.Models
{
    public class PlayerGameResultMessage
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public bool PlayerActive { get; set; }
        public int GameRank { get; set; }
        public int? PointsScored { get; set; }
        public int NemeStatsPointsAwarded { get; set; }
        public int WeightBonusNemePoints { get; set; }
        public int GameDurationBonusNemePoints { get; set; }
        public int TotalNemeStatsPointsAwarded { get; set; }
    }
}
