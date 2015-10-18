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
            string val = GetStringValue(element, attribute, null);
            if (val == null)
                return defaultValue;

            int retVal;
            if (!int.TryParse(val, out retVal))
                retVal = defaultValue;
            return retVal;
        }
        public static bool GetBoolValue(this XElement element, string attribute = null, bool defaultValue = false)
        {
            string val = GetStringValue(element, attribute, null);
            if (val == null)
                return defaultValue;

            int retVal;
            if (!int.TryParse(val, out retVal))
                return defaultValue;

            return retVal == 1;
        }
        public static decimal GetDecimalValue(this XElement element, string attribute = null, decimal defaultValue = -1)
        {
            string val = GetStringValue(element, attribute, null);
            if (val == null)
                return defaultValue;

            decimal retVal;
            if (!decimal.TryParse(val, out retVal))
                return defaultValue;

            return retVal;
        }
        public static bool SetIsExpansion(this XElement Boardgame)
        {
            return (from p in Boardgame.Element("item").Elements("link")
                    where
                        p.Attribute("type").Value == "boardgamecategory" && p.Attribute("id").Value == "1042"
                    select p.Attribute("value").Value).FirstOrDefault() != null;
        }

        public static List<BoardGameLink> SetExpandsLinks(this XElement Boardgame)
        {
            var links = from p in Boardgame.Element("item").Elements("link")
                        where p.Attribute("type").Value == "boardgameexpansion" &&
                            p.Attribute("inbound") != null && p.Attribute("inbound").Value == "true"
                        select new BoardGameLink
                        {
                            Name = p.Attribute("value").Value,
                            GameId = int.Parse(p.Attribute("id").Value)
                        };

            return links.ToList();
        }

        public static List<BoardGameLink> SetExpansionsLinks(this XElement Boardgame)
        {
            var links = from p in Boardgame.Element("item").Elements("link")
                        where p.Attribute("type").Value == "boardgameexpansion" &&
                            (p.Attribute("inbound") == null || p.Attribute("inbound").Value != "true")
                        select new BoardGameLink
                        {
                            Name = p.Attribute("value").Value,
                            GameId = int.Parse(p.Attribute("id").Value)
                        };

            return links.ToList();
        }

        public static List<PlayerPollResult> LoadPlayerPollResults(this XElement xElement)
        {
            List<PlayerPollResult> playerPollResult = new List<PlayerPollResult>();
            if (xElement != null)
            {
                foreach (XElement results in xElement.Elements("results"))
                {
                    PlayerPollResult pResult = new PlayerPollResult()
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
        public static void SetNumplayers(this PlayerPollResult pResult, XElement results)
        {

            string value = results.Attribute("numplayers").Value;
            if (value.Contains("+"))
            {
                pResult.NumPlayersIsAndHigher = true;
            }
            value = value.Replace("+", string.Empty);

            int res = 0;
            int.TryParse(value, out res);

            pResult.NumPlayers = res;
        }
        public static int GetIntResultScore(this XElement results, string selector)
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
            string val = (from p in rankingElement.Elements("rank") where p.Attribute("id").Value == "1" select p.Attribute("value").Value).SingleOrDefault();
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
