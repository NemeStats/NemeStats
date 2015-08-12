using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Models.Badges
{
    public class NemesisBadgeViewModel : IBadgeBaseViewModel
    {
        internal const string FONT_AWESOME_CSS_CLASS = "fa-frown-o";
        internal const string POPOVER_TEXT = "This is the current Player's Nemesis. The Nemesis is the Player with the highest win % vs. this Player. Must have won at least 3 games to be designated as the Nemesis.";

        public string GetFontAwesomeCssClass()
        {
            return FONT_AWESOME_CSS_CLASS;
        }

        public string GetPopoverText()
        {
            return POPOVER_TEXT;
        }
    }
}