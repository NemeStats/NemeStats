using System;

namespace UI.Models.GameDefinitionModels
{
    public class BoardGameGeekGameDefinitionViewModel
    {
        public int? BoardGameGeekGameDefinitionId { get; set; }
        public string Name { get; set; }
        public string Thumbnail { get; set; }
        public int? MaxPlayers { get; set; }
        public int? MinPlayers { get; set; }
        public int? PlayingTime { get; set; }
        public decimal? AverageWeight { get; set; }
        public string Description { get; set; }

        public Uri BoardGameGeekUri { get; set; }
    }
}