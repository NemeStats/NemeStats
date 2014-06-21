using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UI.Models.PlayedGame;

namespace UI.HtmlHelpers
{
    public static class PlayedGameHelper
    {
        internal const string CSS_CLASS_GAME_RANK = "gamerank";
        internal const string PLACE_FIRST = "1st Place";
        internal const string PLACE_SECOND = "2nd Place";
        internal const string PLACE_THIRD = "3rd Place";
        internal const string PLACE_FOURTH = "4th Place";
        internal const string PLACE_BIG_LOSER = "BIG LOSER";
        internal const string CSS_CLASS_FIRST_PLACE = "firstplace";
        internal const string CSS_CLASS_SECOND_PLACE = "secondplace";
        internal const string CSS_CLASS_THIRD_PLACE = "thirdplace";
        internal const string CSS_CLASS_FOURTH_PLACE = "fourthplace";
        internal const string CSS_CLASS_LOSER_PLACE = "loser";

        internal static string HTML_GORDON_POINTS_TEMPLATE = " - ({0} pts.)";
        internal static string HTML_TEMPLATE = "<span class=\"{0} {1}\">{2}{3}</span>";
        
        public static MvcHtmlString GameResults(this HtmlHelper htmlHelper, 
            PlayerGameResultDetails playerGameResultDetails)
        {
            Validate(playerGameResultDetails);

            string cssPlace;
            string gameRankText;

            switch(playerGameResultDetails.GameRank)
            {
                case 1:
                    cssPlace = CSS_CLASS_FIRST_PLACE;
                    gameRankText = PLACE_FIRST;
                    break;
                case 2:
                    cssPlace = CSS_CLASS_SECOND_PLACE;
                    gameRankText = PLACE_SECOND;
                    break;
                case 3:
                    cssPlace = CSS_CLASS_THIRD_PLACE;
                    gameRankText = PLACE_THIRD;
                    break;
                case 4:
                    cssPlace = CSS_CLASS_FOURTH_PLACE;
                    gameRankText = PLACE_FOURTH;
                    break;
                default:
                    cssPlace = CSS_CLASS_LOSER_PLACE;
                    gameRankText = PLACE_BIG_LOSER;
                    break;
            }

            string gordonPointsComponent = string.Format(HTML_GORDON_POINTS_TEMPLATE, playerGameResultDetails.GordonPoints);
            return MvcHtmlString.Create(string.Format(HTML_TEMPLATE,
                CSS_CLASS_GAME_RANK,
                cssPlace,
                gameRankText,
                gordonPointsComponent));
        }

        private static void Validate(PlayerGameResultDetails playerGameResultDetails)
        {
            ValidatePlayerGameResultsDetailsAreNotNull(playerGameResultDetails);
        }

        private static void ValidatePlayerGameResultsDetailsAreNotNull(PlayerGameResultDetails playerGameResultDetails)
        {
            if (playerGameResultDetails == null)
            {
                throw new ArgumentNullException("playerGameResultDetails");
            }
        }
    }
}