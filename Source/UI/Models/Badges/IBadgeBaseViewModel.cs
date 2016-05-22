namespace UI.Models.Badges
{
    public interface IBadgeBaseViewModel
    {
        string GetIconCssClass();
        string GetPopoverText();
    }
}