//
//  Adapted by @vfportero for NemeStats from the bgg-json project created by Erv Walter
//  Original source at https://github.com/ervwalter/bgg-json
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BoardGameGeekApiClient.Helpers;
using BoardGameGeekApiClient.Interfaces;
using BoardGameGeekApiClient.Models;

namespace BoardGameGeekApiClient.Service
{
    public class BoardGameGeekClient : IBoardGameGeekApiClient
    {
        private readonly IApiDownloadService _apiDownloadService;

        public BoardGameGeekClient(IApiDownloadService apiDownloadService)
        {
            _apiDownloadService = apiDownloadService;
        }

        public const string BASE_URL_API_V2 = "http://www.boardgamegeek.com/xmlapi2";

        public GameDetails GetGameDetails(int gameId)
        {

            GameDetails details = null;


            try
            {
                var teamDataURI = new Uri(string.Format(BASE_URL_API_V2 + "/thing?id={0}&stats=1", gameId));
                
                var xDoc = _apiDownloadService.DownloadApiResult(teamDataURI);


                // LINQ to XML.
                var xElements = xDoc.Descendants("items").ToList();
                if (xElements.Count() == 1)
                {

                    var gameCollection = from boardgame in xElements
                                         select new GameDetails
                                         {
                                             Name = boardgame.GetBoardGameName(),
                                             GameId = boardgame.Element("item").GetIntValue("id"),
                                             Artists = boardgame.GetArtists(),
                                             AverageRating =
                                                 boardgame.Element("item")
                                                     .Element("statistics")
                                                     .Element("ratings")
                                                     .Element("average")
                                                     .GetDecimalValue("value"),
                                             BGGRating =
                                                 boardgame.Element("item")
                                                     .Element("statistics")
                                                     .Element("ratings")
                                                     .Element("bayesaverage")
                                                     .GetDecimalValue("value"),
                                             Description = boardgame.Element("item").Element("description").Value,
                                             Designers = boardgame.GetDesigners(),
                                             Expansions = boardgame.GetExpansionsLinks(),
                                             Mechanics = boardgame.GetMechanics(),
                                             Categories = boardgame.GetCategories(),
                                             Image = boardgame.Element("item").Element("image").GetStringValue(),
                                             IsExpansion = boardgame.IsExpansion(),
                                             Thumbnail = boardgame.Element("item").Element("thumbnail").GetStringValue(),
                                             MaxPlayers = boardgame.Element("item").Element("maxplayers").GetIntValue("value"),
                                             MinPlayers = boardgame.Element("item").Element("minplayers").GetIntValue("value"),
                                             PlayerPollResults = boardgame.Element("item").Element("poll").GetPlayerPollResults(),
                                             PlayingTime = boardgame.Element("item").Element("playingtime").GetIntValue("value"),
                                             Publishers = boardgame.GetPublishers(),
                                             Rank =
                                                 boardgame.Element("item")
                                                     .Element("statistics")
                                                     .Element("ratings")
                                                     .Element("ranks")
                                                     .GetRanking(),
                                             YearPublished = boardgame.Element("item").Element("yearpublished").GetIntValue("value"),
                                         };

                    details = gameCollection.FirstOrDefault();

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception on GetGameDetails for ID " + gameId + "." + ex);
            }

            return details;
        }




        public IEnumerable<SearchBoardGameResult> SearchBoardGames(string query, bool exactMatch= false)
        {
            try
            {
                var uriString = string.Format(BASE_URL_API_V2 + "/search?query={0}&type=boardgame", query);
                if (exactMatch)
                {
                    uriString += "&exact=1";
                }
                var searchUrl = new Uri(uriString);
                

                var xDoc = _apiDownloadService.DownloadApiResult(searchUrl);

                // LINQ to XML.
                var gameCollection = xDoc.Descendants("item").Select(boardgame => new SearchBoardGameResult
                {
                    BoardGameName = boardgame.Element("name").GetStringValue("value"),
                    BoardGameId = boardgame.GetIntValue("id"),
                    YearPublished = boardgame.Element("yearpublished").GetIntValue("value")
                });

                if (gameCollection.Any())
                {
                    gameCollection = gameCollection.SortSearchResults(query);
                }
                return gameCollection;

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception on SearchBoardGames for query " + query + "." + ex);
                return new List<SearchBoardGameResult>();
            }
        }






    }
}
