using System;
using BusinessLogic.Models.Achievements;

namespace UI.Models.Achievements
{
    public class AchievementViewModel
    {
        public AchievementViewModel(string name, string description, string fontAwesomeIcon, AchievementLevelEnum achievementLevel)
        {
            Name = name;
            Description = description;
            FontAwesomeIcon = fontAwesomeIcon;
            AchievementLevel = achievementLevel;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string FontAwesomeIcon { get; set; }
        public AchievementLevelEnum AchievementLevel { get; set; }
    }
}