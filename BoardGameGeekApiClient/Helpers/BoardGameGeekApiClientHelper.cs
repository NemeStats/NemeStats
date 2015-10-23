//
//  Adapted by @vfportero for NemeStats from the bgg-json project created by Erv Walter
//  Original source at https://github.com/ervwalter/bgg-json
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using BoardGameGeekApiClient.Models;

namespace BoardGameGeekApiClient.Helpers
{
    public static class BoardGameGeekApiClientHelper
    {
        public static string GetStringValue(this XElement element, string attribute = null, string defaultValue = "")
        {
            if (element == null)
                return defaultValue;

            if (string.IsNullOrEmpty(attribute))
                return element.Value;

            var xatt = element.Attribute(attribute);
            return xatt != null ? xatt.Value : defaultValue;
        }
        public static int GetIntValue(this XElement element, string attribute = null, int defaultValue = -1)
        {
            var val = GetStringValue(element, attribute, null);
            if (string.IsNullOrEmpty(val))
                return defaultValue;

            int retVal;
            if (!int.TryParse(val, out retVal))
                retVal = defaultValue;
            return retVal;
        }

        public static decimal GetDecimalValue(this XElement element, string attribute = null, decimal defaultValue = -1)
        {
            var val = GetStringValue(element, attribute, null);
            if (string.IsNullOrEmpty(val))
                return defaultValue;

            decimal retVal;
            if (!decimal.TryParse(val, out retVal))
                retVal = defaultValue;
            return retVal;
        }

        private static List<string> GetTypeValue(XElement boardgame, string type)
        {
            return (from p in boardgame.Element("item").Elements("link")
                    where p.Attribute("type").Value == type
                    select p.Attribute("value").Value).ToList();
        }

        public static List<string> GetArtists(this XElement boardgame)
        {
            return GetTypeValue(boardgame, "boardgameartist");
        }

        public static List<string> GetDesigners(this XElement boardgame)
        {
            return GetTypeValue(boardgame, "boardgamedesigner");
        }

        public static List<string> GetMechanics(this XElement boardgame)
        {
            return GetTypeValue(boardgame, "boardgamemechanic");
        }
        public static List<string> GetCategories(this XElement boardgame)
        {
            return GetTypeValue(boardgame, "boardgamecategory");
        }

        public static List<string> GetPublishers(this XElement boardgame)
        {
            return GetTypeValue(boardgame, "boardgamepublisher");
        }

        public static string GetBoardGameName(this XElement boardgame)
        {
            return (boardgame.Element("item").Elements("name")
                .Where(p => p.Attribute("type").Value == "primary")
                .Select(p => p.Attribute("value").Value)).SingleOrDefault();
        }
        public static bool IsExpansion(this XElement boardgame)
        {
            return boardgame.Element("item").GetStringValue("type") == "boardgameexpansion";
        }
        public static List<BoardGameLink> GetExpansionsLinks(this XElement Boardgame)
        {
            var links = from p in Boardgame.Element("item").Elements("link")
                        where p.Attribute("type").Value == "boardgameexpansion" &&
                            (p.Attribute("inbound") == null || p.Attribute("inbound").Value != "true")
                        select new BoardGameLink
                        {
                            Name = p.GetStringValue("value"),
                            Id = p.GetIntValue("id")
                        };

            return links.ToList();
        }

        public static List<PlayerPollResult> GetPlayerPollResults(this XElement xElement)
        {
            var playerPollResult = new List<PlayerPollResult>();
            if (xElement != null)
            {
                foreach (var results in xElement.Elements("results"))
                {
                    var pResult = new PlayerPollResult()
                    {
                        Best = GetIntResultScore(results, "Best"),
                        Recommended = GetIntResultScore(results, "Recommended"),
                        NotRecommended = GetIntResultScore(results, "Not Recommended")
                    };
                    SetNumplayers(pResult, results);
                    playerPollResult.Add(pResult);
                }
            }
            return playerPollResult;
        }
        private static void SetNumplayers(this PlayerPollResult pResult, XElement results)
        {

            var value = results.Attribute("numplayers").Value;
            if (value.Contains("+"))
            {
                pResult.NumPlayersIsAndHigher = true;
            }
            value = value.Replace("+", string.Empty);

            var res = 0;
            int.TryParse(value, out res);

            pResult.NumPlayers = res;
        }
        private static int GetIntResultScore(this XElement results, string selector)
        {
            int res = 0;
            try
            {
                string value = (from p in results.Elements("result") where p.Attribute("value").Value == selector select p.Attribute("numvotes").Value).FirstOrDefault();

                if (value != null)
                    int.TryParse(value, out res);
            }
            catch (Exception)
            {
                return 0;
            }

            return res;
        }
        public static int GetRanking(this XElement rankingElement)
        {
            var val = (rankingElement.Elements("rank")
                .Where(p => p.Attribute("id").Value == "1")
                .Select(p => p.Attribute("value").Value)).SingleOrDefault();
            int rank;

            if (val == null)
                rank = -1;
            else if (val.ToLower().Trim() == "not ranked")
                rank = -1;
            else if (!int.TryParse(val, out rank))
                rank = -1;

            return rank;
        }
    }
}
