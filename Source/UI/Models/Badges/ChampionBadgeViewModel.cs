namespace UI.Models.Badges
{
    public class ChampionBadgeViewModel : IBadgeBaseViewModel
    {
        public const string FONT_AWESOME_CSS_CLASS = "fa fa-trophy";
        public const string POPOVER_TEXT = "The Champion is the Player within the Gaming Group with the most wins of this game.";

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