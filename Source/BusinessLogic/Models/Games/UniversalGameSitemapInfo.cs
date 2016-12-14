using System;

namespace BusinessLogic.Models.Games
{
    public class UniversalGameSitemapInfo
    {
        public int BoardGameGeekGameDefinitionId { get; set; }
        public DateTime DateLastGamePlayed { get; set; } = DateTime.MinValue;
        public DateTime DateCreated { get; set; }
    }
}
