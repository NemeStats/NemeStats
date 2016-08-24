//
//  Adapted by @vfportero for NemeStats from the bgg-json project created by Erv Walter
//  Original source at https://github.com/ervwalter/bgg-json
//
using System.Collections.Generic;

namespace BoardGameGeekApiClient.Models
{
    public class GameDetails
    {
        public int GameId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Thumbnail { get; set; }

        public int? MinPlayers { get; set; }
        public int? MaxPlayers { get; set; }
        public int? MaxPlayTime { get; set; }
        public int? MinPlayTime { get; set; }
        public List<string> Mechanics { get; set; }

        public bool IsExpansion { get; set; }
        public int? YearPublished { get; set; }

        public decimal? BGGRating { get; set; }
        public decimal? AverageRating { get; set; }
        public decimal? AverageWeight { get; set; }
        public int Rank { get; set; }

        public List<string> Designers { get; set; }
        public List<string> Publishers { get; set; }
        public List<string> Artists { get; set; }
        public List<GameCategory> Categories { get; set; }
        public List<PlayerPollResult> PlayerPollResults { get; set; }

        public List<BoardGameLink> Expansions { get; set; }
    }
}