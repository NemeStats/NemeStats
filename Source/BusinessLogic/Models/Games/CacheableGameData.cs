namespace BusinessLogic.Models.Games
{
    public class CacheableGameData
    {
        public BoardGameGeekInfo BoardGameGeekInfo { get; set; }
        public int TotalNumberOfGamesPlayed { get; set; }
        public double AveragePlayersPerGame { get; set; }
        public int TotalGamingGroupsWithThisGame { get; set; }
    }
}
