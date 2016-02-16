using System.Linq;

namespace UI.Models.Badges
{
    public class NemesisBadgeViewModel : IBadgeBaseViewModel
    {
        internal const string ICON_CSS_CLASS = "ns-icon-nemesis";
        internal const string POPOVER_TEXT = "This is the current Player's Nemesis. The Nemesis is the Player with the highest win % vs. this Player. Must have won at least 3 games to be designated as the Nemesis.";

        public string GetIconCssClass()
        {
            return ICON_CSS_CLASS;
        }

        public string GetPopoverText()
        {
            return POPOVER_TEXT;
        }
    }
}