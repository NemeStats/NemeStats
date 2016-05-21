using System.Linq;

namespace UI.Models.Badges
{
    public class MinionBadgeViewModel : IBadgeBaseViewModel
    {
        public const string FONT_AWESOME_CSS_CLASS = "ns-icon-minion";
        public const string POPOVER_TEXT = "This is one of the current Player's Minions. Minions are Players who have this Player as a Nemesis.";

        public string GetIconCssClass()
        {
            return FONT_AWESOME_CSS_CLASS;
        }

        public string GetPopoverText()
        {
            return POPOVER_TEXT;
        }
    }
}