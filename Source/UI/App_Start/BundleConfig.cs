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
using System.Web.Optimization;

namespace UI
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include("~/Scripts/jquery-{version}.js")
                .Include("~/Scripts/jquery.cookie.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/moment")
                .Include("~/Scripts/moment.js")
                .Include("~/Scripts/moment.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/javascripts/bootstrap*",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/handlebars")
                .Include("~/Scripts/handlebars.js"));

            bundles.Add(new ScriptBundle("~/bundles/d3")
                .Include("~/Scripts/d3/d3.js")
                .Include("~/Scripts/d3/nv.d3.min.js")
                .Include("~/Scripts/d3/gamesPlayedPieChart.js")
                .Include("~/Scripts/d3/nemeStatsPointsLineGraph.js"));

            bundles.Add(new StyleBundle("~/bundles/d3/css")
                .Include("~/Scripts/d3/css/nv.d3.css"));

            bundles.Add(new ScriptBundle("~/bundles/custom")
                .Include("~/Scripts/namespace-{version}.js")
                .Include("~/Scripts/Plugins/toEditBoxPlugin.js")
                .Include("~/Scripts/Plugins/rankPlugin.js")
                .Include("~/Scripts/GamingGroup/gamingGroup.js")
                .Include("~/Scripts/CreatePlayedGame/createplayedgame.js")
                .Include("~/Scripts/PlayedGame/search.js")
                .Include("~/Scripts/PlayedGame/recordexceldownload.js")
                .Include("~/Scripts/Player/createOrUpdatePlayer.js")
                .Include("~/Scripts/Player/playerDetails.js")
                .Include("~/Scripts/Player/players.js")
                .Include("~/Scripts/GameDefinition/gameDefinitionAutoComplete.js")
                .Include("~/Scripts/GameDefinition/createGameDefinitionPartial.js")
                .Include("~/Scripts/GameDefinition/createGameDefinition.js")
                .Include("~/Scripts/GameDefinition/gameDefinitions.js")
                .Include("~/Scripts/Shared/_Layout.js")
                .Include("~/Scripts/Shared/_LoginPartial.js")
                .Include("~/Scripts/Shared/GoogleAnalytics.js"));

            bundles.Add(new StyleBundle("~/bundles/content/css")
                .Include("~/css/bootstrap.css")
                .Include("~/css/theme.css")
                .Include("~/css/nemestats.css")
                .Include("~/css/fonts.css")
                );

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                "~/Content/Themes/base/jquery-ui.css"));
        }
    }
}
