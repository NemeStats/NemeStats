#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion
using System;
using System.Web.Mvc;
using BusinessLogic.Models.PlayedGames;
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
        internal const string PLACE_TEAM_WIN = "EVERYONE WON";
        internal const string PLACE_EVERYONE_LOST = "EVERYONE LOST";
        internal const string CSS_CLASS_FIRST_PLACE = "firstplace";
        internal const string CSS_CLASS_SECOND_PLACE = "secondplace";
        internal const string CSS_CLASS_THIRD_PLACE = "thirdplace";
        internal const string CSS_CLASS_FOURTH_PLACE = "fourthplace";
        internal const string CSS_CLASS_LOSER_PLACE = "loser";
        internal const string CSS_CLASS_TEAM_WIN = "gameResult-teamWin";
        internal const string CSS_CLASS_TEAM_LOSS = "gameResult-teamLoss";
        internal const string NEMEPOINTICO_TOOLTIP = "NemePoints earned in this played game";
        internal const string NEMEPOINTSICO_TOOLTIP_POSITION = "top";

        internal static string HTML_NEME_POINTS_TEMPLATE = " - ({0} {1})";
        internal static string HTML_TEMPLATE = "<span class=\"{0} {1}\">{2}{3}</span>";


        public static MvcHtmlString GameResults(this HtmlHelper htmlHelper, GameResultViewModel playerGameResultDetails, WinnerTypes? winnerType)
        {
            Validate(playerGameResultDetails);

            string cssPlace;
            string gameRankText;

            if (winnerType == WinnerTypes.TeamWin)
            {
                cssPlace = CSS_CLASS_TEAM_WIN;
                gameRankText = PLACE_TEAM_WIN;
            }
            else if (winnerType == WinnerTypes.TeamLoss)
            {
                cssPlace = CSS_CLASS_TEAM_LOSS;
                gameRankText = PLACE_EVERYONE_LOST;
            }
            else
            {
                switch (playerGameResultDetails.GameRank)
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
            }


            string nemeStatsPointsComponent = string.Format(HTML_NEME_POINTS_TEMPLATE, playerGameResultDetails.NemePointsSummary.TotalNemePoints, htmlHelper.NemePointsIco(showTooltip: true, tooltip: NEMEPOINTICO_TOOLTIP, tooltipPosition: NEMEPOINTSICO_TOOLTIP_POSITION));
            return MvcHtmlString.Create(string.Format(HTML_TEMPLATE,
                CSS_CLASS_GAME_RANK,
                cssPlace,
                gameRankText,
                nemeStatsPointsComponent));
        }

        private static void Validate(GameResultViewModel playerGameResultDetails)
        {
            ValidatePlayerGameResultsDetailsAreNotNull(playerGameResultDetails);
        }

        private static void ValidatePlayerGameResultsDetailsAreNotNull(GameResultViewModel playerGameResultDetails)
        {
            if (playerGameResultDetails == null)
            {
                throw new ArgumentNullException("playerGameResultDetails");
            }
        }
    }
}