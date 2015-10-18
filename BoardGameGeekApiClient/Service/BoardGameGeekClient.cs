//
//  Adapted by @vfportero for NemeStats from the bgg-json project created by Erv Walter
//  Original source at https://github.com/ervwalter/bgg-json
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BoardGameGeekApiClient.Helpers;
using BoardGameGeekApiClient.Interfaces;
using BoardGameGeekApiClient.Models;

namespace BoardGameGeekApiClient.Service
{
    public class BoardGameGeekClient : IBoardGameGeekApiClient
    {

        public const string BASE_URL_API_V2 = "http://www.boardgamegeek.com/xmlapi2";

        public async Task<GameDetails> GetGameDetails(int gameId)
        {

            GameDetails details = null;


            try
            {
                var teamDataURI = new Uri(string.Format(BASE_URL_API_V2 + "/thing?id={0}&stats=1", gameId));
                var xDoc = await ReadData(teamDataURI);

                
                // LINQ to XML.
                var xElements = xDoc.Descendants("items");
                if (xElements.Count() != 1)
                {
                    return null;
                }
                var gameCollection = from boardgame in xElements
                                                          select new GameDetails
                                                          {
                                                              Name = boardgame.GetBoardGameName(),
                                                              GameId = boardgame.Element("item").GetIntValue("id"),
                                                              Artists = boardgame.GetArtists(),
                                                              AverageRating = boardgame.Element("item").Element("statistics").Element("ratings").Element("average").GetDecimalValue("value"),
                                                              BGGRating = boardgame.Element("item").Element("statistics").Element("ratings").Element("bayesaverage").GetDecimalValue("value"),
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
                                                              Rank = boardgame.Element("item").Element("statistics").Element("ratings").Element("ranks").GetRanking(),
                                                              YearPublished = boardgame.Element("item").Element("yearpublished").GetIntValue("value"),
                                                          };

                details = gameCollection.FirstOrDefault();

               
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception on GetGameDetails for ID " + gameId + "." + ex);
            }

            return details;
        }

     


        public async Task<IEnumerable<SearchBoardGameResult>> SearchBoardGames(string query)
        {
            try
            {
                var teamDataURI = new Uri(string.Format(BASE_URL_API_V2 + "/search?query={0}&type=boardgame", query));

                var xDoc = await ReadData(teamDataURI);

                // LINQ to XML.
                var gameCollection = xDoc.Descendants("item").Select(boardgame => new SearchBoardGameResult
                {
                    Name = boardgame.Element("name").GetStringValue("value"),
                    GameId = boardgame.GetIntValue("id"),
                    YearPublished = boardgame.Element("yearpublished").GetIntValue("value")
                });
                return gameCollection;

            }
            catch (Exception ex)
            {
                throw;
                //return new List<SearchBoardGameResult>();
            }
        }


        private async Task<XDocument> ReadData(Uri requestUrl)
        {
            Debug.WriteLine("Downloading " + requestUrl);
            // Due to malformed header I cannot use GetContentAsync and ReadAsStringAsync :(
            // UTF-8 is now hard-coded...

            XDocument data = null;
            int retries = 0;
            while (data == null && retries < 60)
            {
                retries++;
                var request = WebRequest.CreateHttp(requestUrl);
                request.Timeout = 15000;
                using (var response = (HttpWebResponse)(await request.GetResponseAsync()))
                {
                    if (response.StatusCode == HttpStatusCode.Accepted)
                    {
                        await Task.Delay(500);
                        continue;
                    }
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        data = XDocument.Parse(await reader.ReadToEndAsync());
                    }
                }
            }

            if (data != null)
            {
                return data;
            }
            else
            {
                throw new Exception("Failed to download BGG data.");
            }

        }

       
    }
}
