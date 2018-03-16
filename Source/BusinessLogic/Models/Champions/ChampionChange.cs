using System;

namespace BusinessLogic.Models.Champions
{
    public class ChampionChange
    {
        public string PreviousChampionPlayerName { get; set; }
        public string NewChampionPlayerName { get; set; }
        public string GameName { get; set; }
        public DateTime DateCreated { get; set; }
        public int? PreviousChampionPlayerId { get; set; }
        public int NewChampionPlayerId { get; set; }
        public int GameDefinitionId { get; set; }
    }
}
