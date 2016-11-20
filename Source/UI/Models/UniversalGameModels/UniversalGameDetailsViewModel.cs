using UI.Models.GameDefinitionModels;

namespace UI.Models.UniversalGameModels
{
    public class UniversalGameDetailsViewModel
    {
        public BoardGameGeekInfoViewModel BoardGameGeekInfo { get; set; }
        public int TotalNumberOfGamesPlayed { get; set; }
        public string AveragePlayersPerGame { get; set; }
        public int TotalGamingGroupsWithThisGame { get; set; }
        public GamingGroupGameDefinitionViewModel GamingGroupGameDefinitionSummary { get; set; }
    }
}