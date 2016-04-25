using System;

namespace UI.Models.Achievements
{
    public class AchievementViewModel
    {
        public AchievementViewModel(string name, string description, string fontAwesomeIcon)
        {
            Name = name;
            Description = description;
            FontAwesomeIcon = fontAwesomeIcon;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string FontAwesomeIcon { get; set; }
    }
}