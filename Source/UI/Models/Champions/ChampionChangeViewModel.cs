using System;

namespace UI.Models.Champions
{
    public class ChampionChangeViewModel
    {
        public string PreviousChampionPlayerName { get; set; }
        public string NewChampionPlayerName { get; set; }
        public string GameName { get; set; }
        public DateTime DateCreated { get; set; }
        public int PreviousChampionPlayerId { get; set; }
        public int NewChampionPlayerId { get; set; }
        public int GameDefinitionId { get; set; }
    }
}