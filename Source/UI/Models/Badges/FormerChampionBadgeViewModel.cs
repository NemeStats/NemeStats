namespace UI.Models.Badges
{
    public class FormerChampionBadgeViewModel : IBadgeBaseViewModel
    {
        internal const string FONT_AWESOME_CSS_CLASS = "ns-icon-former-champion";
        internal const string POPOVER_TEXT = "This Player is the former Champion of this game.";

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