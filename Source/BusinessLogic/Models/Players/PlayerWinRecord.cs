namespace BusinessLogic.Models.Players
{
    public class PlayerWinRecord
    {
        public string PlayerName { get; set; }
        public int PlayerId { get; set; }
        public bool PlayerActive { get; set; }
        public int GamesWon { get; set; }
        public int GamesLost { get; set; }
        public int WinPercentage { get; set; }
        public bool IsChampion { get; set; }
        public bool IsFormerChampion { get; set; }
        public int TotalGamesPlayed { get; set; }
        public int BaseNemePoints { get; set; }
        public int WeightBonusNemePoints { get; set; }
        public int GameDurationBonusNemePoints { get; set; }
        public float AveragePointsPerGame { get; set; }
    }
}
