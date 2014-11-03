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

namespace BusinessLogic.Logic
{
    public class BoardGameGeekSearcher : IBoardGameGeekSearcher
    {
        public const string URI_FORMAT_BOARD_GAME_GEEK_SEARCH_REQUEST = "http://www.boardgamegeek.com/xmlapi/search?search={0}";

        public List<BoardGameGeekSearchResult> SearchForBoardGames(string searchText)
        {
            List<BoardGameGeekSearchResult> searchResults = new List<BoardGameGeekSearchResult>();

            var httpRequest = BuildHttpRequest(searchText);
            using (var webResponse = (HttpWebResponse)httpRequest.GetResponse())
            {
                ValidateHttpResponseStatusCode(webResponse);

                BuildSearchResults(webResponse, searchResults);
            }

            return searchResults;
        }

        private static HttpWebRequest BuildHttpRequest(string searchText)
        {
            Uri boardGameGeekApiUri = new Uri(string.Format(URI_FORMAT_BOARD_GAME_GEEK_SEARCH_REQUEST, HttpUtility.UrlEncode(searchText)));
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

            BoardGameGeekSearchResult individualSearchResult = new BoardGameGeekSearchResult();
            foreach (var searchResult in searchResults.boardgame)
            {
                VetSearchResult(searchResult, individualSearchResult, returnValue);
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

        private static void VetSearchResult(boardgamesBoardgame searchResult, BoardGameGeekSearchResult individualSearchResult, List<BoardGameGeekSearchResult> returnValue)
        {
            foreach (var gameName in searchResult.name)
            {
                if (gameName.primary != "true")
                {
                    continue;
                }
                individualSearchResult.BoardGameName = gameName.Value;
                individualSearchResult.BoardGameId = searchResult.objectid;
                individualSearchResult.YearPublished = searchResult.yearpublished;
                returnValue.Add(individualSearchResult);
                break;
            }
        }
    }
}