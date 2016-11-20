using UI.Models.GameDefinitionModels;

namespace UI.Models.UniversalGameModels
{
    public class UniversalGameDetailsViewModel
    {
        public BoardGameGeekInfoViewModel BoardGameGeekInfo { get; set; }
        public UniversalGameStatsViewModel UniversalGameStats { get; set; }
        public GamingGroupGameDefinitionViewModel GamingGroupGameDefinitionSummary { get; set; }
    }
}