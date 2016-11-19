using System.Collections.Generic;

namespace BusinessLogic.Models.Games
{
    public class CacheableGameData
    {
        public int BoardGameGeekGameDefinitionId { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string ThumbnailImageUrl { get; set; }
        public int? MaxPlayers { get; set; }
        public int? MinPlayers { get; set; }
        public int? MinPlayTime { get; set; }
        public int? MaxPlayTime { get; set; }
        public decimal? BoardGameGeekAverageWeight { get; set; }
        public int? BoardGameGeekYearPublished { get; set; }
        public List<string> BoardGameGeekCategories { get; set; }
        public List<string> BoardGameGeekMechanics { get; set; }
        public string BoardGameGeekDescription { get; set; }
        public int TotalNumberOfGamesPlayed { get; set; }
        public double AveragePlayersPerGame { get; set; }
        public int TotalGamingGroupsWithThisGame { get; set; }
    }
}
