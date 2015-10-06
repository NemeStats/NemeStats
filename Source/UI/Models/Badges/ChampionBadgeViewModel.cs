using System.Linq;

namespace UI.Models.Badges
{
    public class ChampionBadgeViewModel : IBadgeBaseViewModel
    {
        internal const string FONT_AWESOME_CSS_CLASS = "fa-trophy";
        internal const string POPOVER_TEXT = "The Champion is the Player within the Gaming Group with the most wins of this game.";

        public string GameName { get; set; }
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