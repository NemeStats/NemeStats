namespace BusinessLogic.Models.Games
{
    public class BoardGameGeekGameSummary
    {
        public BoardGameGeekInfo BoardGameGeekInfo { get; set; }
        public int TotalNumberOfGamesPlayed { get; set; }
        public double AveragePlayersPerGame { get; set; }
        public int TotalGamingGroupsWithThisGame { get; set; }
        public GameDefinitionSummary GamingGroupGameDefinitionSummary { get; set; }
    }
}
