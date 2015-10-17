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
                                                              Name = (from p in boardgame.Element("item").Elements("name") where p.Attribute("type").Value == "primary" select p.Attribute("value").Value).SingleOrDefault(),
                                                              GameId = int.Parse(boardgame.Element("item").Attribute("id").Value),
                                                              Artists = (from p in boardgame.Element("item").Elements("link") where p.Attribute("type").Value == "boardgameartist" select p.Attribute("value").Value).ToList(),
                                                              AverageRating = decimal.Parse(boardgame.Element("item").Element("statistics").Element("ratings").Element("average").Attribute("value").Value),
                                                              BGGRating = decimal.Parse(boardgame.Element("item").Element("statistics").Element("ratings").Element("bayesaverage").Attribute("value").Value),
                                                              Description = boardgame.Element("item").Element("description").Value,
                                                              Designers = (from p in boardgame.Element("item").Elements("link") where p.Attribute("type").Value == "boardgamedesigner" select p.Attribute("value").Value).ToList(),
                                                              Expands = boardgame.SetExpandsLinks(),
                                                              Expansions = boardgame.SetExpansionsLinks(),
                                                              Mechanics = (from p in boardgame.Element("item").Elements("link") where p.Attribute("type").Value == "boardgamemechanic" select p.Attribute("value").Value).ToList(),
                                                              Image = boardgame.Element("item").Element("image") != null ? boardgame.Element("item").Element("image").Value : string.Empty,
                                                              IsExpansion = boardgame.SetIsExpansion(),
                                                              Thumbnail = boardgame.Element("item").Element("thumbnail") != null ? boardgame.Element("item").Element("thumbnail").Value : string.Empty,
                                                              MaxPlayers = int.Parse(boardgame.Element("item").Element("maxplayers").Attribute("value").Value),
                                                              MinPlayers = int.Parse(boardgame.Element("item").Element("minplayers").Attribute("value").Value),
                                                              PlayerPollResults = boardgame.Element("item").Element("poll").LoadPlayerPollResults(),
                                                              PlayingTime = int.Parse(boardgame.Element("item").Element("playingtime").Attribute("value").Value),
                                                              Publishers = (from p in boardgame.Element("item").Elements("link") where p.Attribute("type").Value == "boardgamepublisher" select p.Attribute("value").Value).ToList(),
                                                              Rank = boardgame.Element("item").Element("statistics").Element("ratings").Element("ranks").GetRanking(),
                                                              YearPublished = int.Parse(boardgame.Element("item").Element("yearpublished").Attribute("value").Value)
                                                          };

                details = gameCollection.FirstOrDefault();

                if (details.Expands != null && details.Expands.Count == 0)
                {
                    details.Expands = null;
                }
                if (details.Expansions != null && details.Expansions.Count == 0)
                {
                    details.Expansions = null;
                }

                return details;
            }
            catch (Exception ex)
            {
                return null;
            }
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
