using System.Collections.Generic;
using BusinessLogic.Models.Champions;

namespace BusinessLogic.Models.Games
{
    public class BoardGameGeekGameSummary
    {
        public BoardGameGeekInfo BoardGameGeekInfo { get; set; }
        public UniversalGameStats UniversalGameStats { get; set; }
        public GameDefinitionSummary GamingGroupGameDefinitionSummary { get; set; }
        public List<PublicGameSummary> RecentlyPlayedGames { get; set; }
        public List<ChampionData> TopChampions { get; set; }
    }
}
