//
//  Adapted by @vfportero for NemeStats from the bgg-json project created by Erv Walter
//  Original source at https://github.com/ervwalter/bgg-json
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using BoardGameGeekApiClient.Helpers;
using BoardGameGeekApiClient.Interfaces;
using BoardGameGeekApiClient.Models;
using Exceptionless;
using RollbarSharp;

namespace BoardGameGeekApiClient.Service
{
    public class BoardGameGeekClient : IBoardGameGeekApiClient
    {
        private readonly IApiDownloadService _apiDownloadService;
        private readonly IRollbarClient _rollbar;


        public BoardGameGeekClient(IApiDownloadService apiDownloadService, IRollbarClient rollbarClient)
        {
            _apiDownloadService = apiDownloadService;
            _rollbar = rollbarClient;
        }

        public const string BASE_URL_API_V2 = "http://www.boardgamegeek.com/xmlapi2";

        public string GetGameThumbnail(int gameId)
        {
            var thumbnail = "";
            try
            {
                var apiUri = new Uri(BASE_URL_API_V2 + $"/thing?id={gameId}");
                var xDoc = _apiDownloadService.DownloadApiResult(apiUri);

                var xElements = xDoc.Descendants("items").ToList();
                if (xElements.Count == 1)
                {
                    var firstImageElement = xElements.First().Element("item").Element("image");
                    if (firstImageElement != null)
                    {
                        thumbnail = firstImageElement.GetStringValue();
                    }
                }
            }
            catch (Exception ex)
            {
                _rollbar.SendException(ex);
                ex.ToExceptionless();
            }

            return thumbnail;
        }

        public GameDetails GetGameDetails(int gameId)
        {

            GameDetails details = null;


            try
            {
                var apiUri = new Uri(BASE_URL_API_V2 + $"/thing?id={gameId}&stats=1");

                var xDoc = _apiDownloadService.DownloadApiResult(apiUri);


                // LINQ to XML.
                var xElements = xDoc.Descendants("items").ToList();
                if (xElements.Count == 1)
                {

                    var gameCollection = xElements.Select(boardgame => new GameDetails
                    {
                        Name = boardgame.Element("item").GetBoardGameName(),
                        GameId = boardgame.Element("item").GetIntValue("id", gameId).Value,
                        Artists = boardgame.Element("item").GetArtists(),
                        AverageRating =
                            boardgame.Element("item")
                                .Element("statistics")
                                .Element("ratings")
                                .Element("average")
                                .GetDecimalValue("value"),
                        AverageWeight = 
                            boardgame.Element("item")
                                .Element("statistics")
                                .Element("ratings")
                                .Element("averageweight")
                                .GetDecimalValue("value"),
                        BGGRating =
                            boardgame.Element("item")
                                .Element("statistics")
                                .Element("ratings")
                                .Element("bayesaverage")
                                .GetDecimalValue("value"),
                        Description = boardgame.Element("item").Element("description").Value,
                        Designers = boardgame.Element("item").GetDesigners(),
                        Expansions = boardgame.Element("item").GetExpansionsLinks(),
                        Mechanics = boardgame.Element("item").GetMechanics(),
                        Categories = boardgame.Element("item").GetCategories(),
                        Image = boardgame.Element("item").Element("image").GetStringValue(),
                        IsExpansion = boardgame.Element("item").IsExpansion(),
                        Thumbnail = boardgame.Element("item").Element("thumbnail").GetStringValue(),
                        MaxPlayers = boardgame.Element("item").Element("maxplayers").GetIntValue("value"),
                        MinPlayers = boardgame.Element("item").Element("minplayers").GetIntValue("value"),
                        PlayerPollResults = boardgame.Element("item").Element("poll").GetPlayerPollResults(),
                        MaxPlayTime = boardgame.Element("item").Element("maxplaytime").GetIntValue("value"),
                        MinPlayTime = boardgame.Element("item").Element("minplaytime").GetIntValue("value"),
                        Publishers = boardgame.Element("item").GetPublishers(),
                        Rank =
                            boardgame.Element("item")
                                .Element("statistics")
                                .Element("ratings")
                                .Element("ranks")
                                .GetRanking(),
                        YearPublished = boardgame.Element("item").Element("yearpublished").GetIntValue("value"),
                    });

                    details = gameCollection.FirstOrDefault();

                }
            }
            catch (Exception ex)
            {
                _rollbar.SendException(ex);
                ex.ToExceptionless();
            }

            return details;
        }

        public UserDetails GetUserDetails(string userName)
        {
            UserDetails details = null;


            try
            {
                var teamDataURI = new Uri(string.Format(BASE_URL_API_V2 + "/user?name={0}", userName));

                var xDoc = _apiDownloadService.DownloadApiResult(teamDataURI);


                var xElements = xDoc.Descendants("user").ToList();
                if (xElements.Count == 1)
                {

                    var userElement = xElements.First();
                    int userId;
                    if (int.TryParse(userElement.Attribute("id").Value, out userId))
                    {
                        var avatarLink = userElement.Element("avatarlink").GetStringValue(attribute: "value");
                        Uri avatarUri;
                        if (!Uri.TryCreate(avatarLink, UriKind.Absolute, out avatarUri))
                        {
                            avatarLink = string.Empty;
                        }
                        details = new UserDetails
                        {
                            Name = userName,
                            Avatar = avatarLink,
                            UserId = userId
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _rollbar.SendException(ex);
                ex.ToExceptionless();
            }

            return details;
        }

        public List<GameDetails> GetUserGames(string userName)
        {
            try
            {
                var uriString = $"{BASE_URL_API_V2}/collection?username={userName}&own=1";
                var url = new Uri(uriString);

                var xDoc = _apiDownloadService.DownloadApiResult(url);

                var xElements = xDoc.Descendants("items").ToList();
                if (xElements.Count == 1)
                {
                    var gameCollection = xElements.Descendants("item").Select(boardgame => new GameDetails
                    {
                        Name = boardgame.Element("name").GetStringValue(),
                        YearPublished = boardgame.Element("yearpublished").GetIntValue(),
                        GameId = boardgame.GetIntValue("objectid",-1).Value,
                        Image = boardgame.Element("image").GetStringValue(),
                        Thumbnail = boardgame.Element("thumbnail").GetStringValue(),
                        IsExpansion = boardgame.IsExpansion("subtype"),
                    });

                    return gameCollection.Where(g => !g.IsExpansion).ToList();
                }
            }

            catch (Exception ex)
            {
                _rollbar.SendException(ex);
                ex.ToExceptionless();
            }

            return new List<GameDetails>();
        }

        private string CleanUpQuery(string query)
        {
            return Uri.EscapeUriString(query.Trim());
        }

        public List<SearchBoardGameResult> SearchBoardGames(string query, bool exactMatch = false)
        {
            try
            {
                var uriString = BASE_URL_API_V2 + $"/search?query={CleanUpQuery(query)}&type=boardgame";
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
                    BoardGameId = boardgame.GetIntValue("id",-1).Value,
                    YearPublished = boardgame.Element("yearpublished").GetIntValue("value",-1).Value
                }).ToList();

                if (gameCollection.Any())
                {
                    gameCollection = gameCollection.SortSearchResults(query);
                }
                return gameCollection;

            }
            catch (Exception ex)
            {
                _rollbar.SendException(ex);
                ex.ToExceptionless();
                return new List<SearchBoardGameResult>();
            }
        }






    }
}
