using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using BusinessLogic.Logic.BoardGameGeek;
using BusinessLogic.Models.Games;

namespace BusinessLogic.Logic.BoardGameGeek
{
    public class BoardGameGeekSearcher : IBoardGameGeekSearcher
    {
        public const string URI_FORMAT_BOARD_GAME_GEEK_SEARCH_REQUEST = "http://www.boardgamegeek.com/xmlapi/search?search={0}";
        public const string URI_FRAGMENT_EXACT_MATCH = "&exact=1";
        public const int MAX_RESULTS_TO_RETURN = 10;

        public List<BoardGameGeekSearchResult> SearchForBoardGames(string searchText, bool exactMatch)
        {
            searchText = searchText.Trim();
            var searchResults = new List<BoardGameGeekSearchResult>();

            var httpRequest = BuildHttpRequest(searchText, exactMatch);
            using (var webResponse = (HttpWebResponse)httpRequest.GetResponse())
            {
                ValidateHttpResponseStatusCode(webResponse);

                BuildSearchResults(webResponse, searchResults);
            }

            return SortSearchResults(searchText, searchResults).Take(MAX_RESULTS_TO_RETURN).ToList();
        }

        private static HttpWebRequest BuildHttpRequest(string searchText, bool exactMatch)
        {
            string uri = string.Format(URI_FORMAT_BOARD_GAME_GEEK_SEARCH_REQUEST, HttpUtility.UrlEncode(searchText));
            if (exactMatch)
            {
                uri += URI_FRAGMENT_EXACT_MATCH;
            }
            Uri boardGameGeekApiUri = new Uri(uri);
            HttpWebRequest httpRequest = WebRequest.CreateHttp(boardGameGeekApiUri);
            httpRequest.Method = "GET";
            return httpRequest;
        }

        private static void ValidateHttpResponseStatusCode(HttpWebResponse webResponse)
        {
            if (webResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new HttpException((int)webResponse.StatusCode,
                                        "Google Analytics tracking did not return OK 200");
            }
        }

        private static void BuildSearchResults(HttpWebResponse webResponse, List<BoardGameGeekSearchResult> returnValue)
        {
            var searchResults = GetBoardGameGeekSearchResults(webResponse);

            if (searchResults.boardgame == null)
            {
                return;
            }

            foreach (var searchResult in searchResults.boardgame)
            {
                VetSearchResult(searchResult, returnValue);
            }
        }

        private static boardgames GetBoardGameGeekSearchResults(HttpWebResponse webResponse)
        {
            boardgames searchResult;
            using (StreamReader streamReader = new StreamReader(webResponse.GetResponseStream(), System.Text.Encoding.UTF8))
            {
                string result = streamReader.ReadToEnd();

                XmlSerializer serializer = new XmlSerializer(typeof(boardgames));
                MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(result));
                searchResult = (boardgames)serializer.Deserialize(memStream);
            }
            return searchResult;
        }

        private static void VetSearchResult(boardgamesBoardgame searchResult,  List<BoardGameGeekSearchResult> returnValue)
        {
            BoardGameGeekSearchResult individualSearchResult;
            foreach (var gameName in searchResult.name)
            {
                if (gameName.primary != "true")
                {
                    continue;
                }
                individualSearchResult = new BoardGameGeekSearchResult();
                individualSearchResult.BoardGameName = gameName.Value;
                individualSearchResult.BoardGameId = int.Parse(searchResult.objectid);
                individualSearchResult.YearPublished = searchResult.yearpublished;
                returnValue.Add(individualSearchResult);
                break;
            }
        }

        internal static List<BoardGameGeekSearchResult> SortSearchResults(string searchText, List<BoardGameGeekSearchResult> searchResults)
        {
            var queryable = searchResults.AsQueryable();
            var results = new List<BoardGameGeekSearchResult>();
            //first add exact matches
            results.AddRange(queryable.Where(result => result.BoardGameName.Equals(searchText, StringComparison.InvariantCultureIgnoreCase))
                .OrderByDescending(result => result.YearPublished)
                .Take(MAX_RESULTS_TO_RETURN));

            if (results.Count >= MAX_RESULTS_TO_RETURN)
            {
                return results;
            }

            //then add anything that starts with but isn't an exact match
            results.AddRange(queryable.Where(result => result.BoardGameName.StartsWith(searchText, StringComparison.InvariantCultureIgnoreCase)
                    && !result.BoardGameName.Equals(searchText, StringComparison.InvariantCultureIgnoreCase))
                    .OrderByDescending(result => result.YearPublished)
                .Take(MAX_RESULTS_TO_RETURN));

            if (results.Count >= MAX_RESULTS_TO_RETURN)
            {
                return results;
            }
            //then add everything else
            results.AddRange(queryable.Where(result => !result.BoardGameName.StartsWith(searchText, StringComparison.InvariantCultureIgnoreCase)
                && !result.BoardGameName.Equals(searchText, StringComparison.InvariantCultureIgnoreCase))
                .OrderByDescending(result => result.YearPublished)
                .Take(MAX_RESULTS_TO_RETURN));

            return results;
        }
    }
}