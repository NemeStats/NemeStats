using System.Linq;

namespace UI.Models.Badges
{
    public interface IBadgeBaseViewModel
    {
        string GetFontAwesomeCssClass();
        string GetPopoverText();
    }
}