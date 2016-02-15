namespace UI.Models.Badges
{
    public class PreviousNemesisBadgeViewModel : IBadgeBaseViewModel
    {
        internal const string FONT_AWESOME_CSS_CLASS = "fa-user-times";
        internal const string POPOVER_TEXT = "This Player used to be the Nemesis of the current Player.";

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