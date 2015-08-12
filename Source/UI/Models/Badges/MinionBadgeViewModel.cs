using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Models.Badges
{
    public class MinionBadgeViewModel : IBadgeBaseViewModel
    {
        internal const string FONT_AWESOME_CSS_CLASS = "fa-thumbs-o-up";
        internal const string POPOVER_TEXT = "This is one of the current Player's Minions. Minions are Players who have this Player as a Nemesis.";

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