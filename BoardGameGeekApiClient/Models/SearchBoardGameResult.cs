//
//  Adapted by @vfportero for NemeStats from the bgg-json project created by Erv Walter
//  Original source at https://github.com/ervwalter/bgg-json
//
namespace BoardGameGeekApiClient.Models
{
    public class SearchBoardGameResult
    {
        public string Name { get; set; }
        public int GameId { get; set; }
        public int YearPublished { get; set; }
    }
}