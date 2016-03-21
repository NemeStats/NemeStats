namespace UI.Areas.Api.Models
{
    public class PlayerStatisticsMessage
    {
        public int TotalGames { get; set; }
        public int TotalPoints { get; set; }
        public int BaseNemePoints { get; set; }
        public int WeightBonusNemePoints { get; set; }
        public int GameDurationBonusNemePoints { get; set; }
        public float AveragePlayersPerGame { get; set; }
        public int TotalGamesWon { get; set; }
        public int TotalGamesLost { get; set; }
        public int WinPercentage { get; set; }
        public GameDefinitionTotalsMessage GameDefinitionTotals { get; set; }
    }
}
