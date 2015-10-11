namespace UI.Models.Badges
{
    public class FormerChampionBadgeViewModel : IBadgeBaseViewModel
    {
        internal const string FONT_AWESOME_CSS_CLASS = "fa-trophy fa-slashed";
        internal const string POPOVER_TEXT = "This Player is the former Champion of this game.";

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