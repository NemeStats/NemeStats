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
using System.Web.Mvc;

namespace UI.HtmlHelpers
{
    public static class UIHelper
    {
        internal const string NEMEPOINTICO_CSS_CLASS = "neme-points-ico";
        public static MvcHtmlString NemePointsIco(this HtmlHelper htmlHelper, string tooltip = "", string tooltipPosition = "top")
        {
            var tootlipHtml = !string.IsNullOrEmpty(tooltip)
                ? $"data-toggle=\"popover\" data-placement=\"{tooltipPosition}\" data-content=\"{tooltip}\""
                : "";

            var html =
                $"<span class=\"fa-stack {NEMEPOINTICO_CSS_CLASS}\" {tootlipHtml}><i class=\"fa fa-circle fa-stack-2x\"></i><i class=\"fa fa-stack-1x letter\">N</i></span>";

            return new MvcHtmlString(html);

        }
    }
}