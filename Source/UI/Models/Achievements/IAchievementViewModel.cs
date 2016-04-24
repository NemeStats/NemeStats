using System;

namespace UI.Models.Achievements
{
    public interface IAchievementViewModel
    {
        Uri ImageUrl { get; set; }
        string ToolTip { get; set; }
    }
}