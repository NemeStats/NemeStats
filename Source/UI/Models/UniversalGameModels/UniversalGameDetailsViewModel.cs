using System.Collections.Generic;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.Players;
using UI.Models.GameDefinitionModels;

namespace UI.Models.UniversalGameModels
{
    public class UniversalGameDetailsViewModel
    {
        public BoardGameGeekInfoViewModel BoardGameGeekInfo { get; set; }
        public UniversalGameStatsViewModel UniversalGameStats { get; set; }
        public GamingGroupGameDefinitionViewModel GamingGroupGameDefinitionSummary { get; set; }
        public List<PublicGameSummary> RecentlyPlayedGames { get; set; }
        public List<ChampionDataModel> TopChampions { get; set; }
    }
}