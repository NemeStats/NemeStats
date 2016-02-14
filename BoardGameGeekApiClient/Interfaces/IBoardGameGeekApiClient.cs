//
//  Adapted by @vfportero for NemeStats from the bgg-json project created by Erv Walter
//  Original source at https://github.com/ervwalter/bgg-json
//

using System.Collections.Generic;
using BoardGameGeekApiClient.Models;

namespace BoardGameGeekApiClient.Interfaces
{
    public interface IBoardGameGeekApiClient
    {
        List<SearchBoardGameResult> SearchBoardGames(string query, bool exactMatch = false);
        GameDetails GetGameDetails(int gameId);
        UserDetails GetUserDetails(string userName);
        List<GameDetails> GetUserGames(string userName);
        string GetGameThumbnail(int gameId);
    }
}
