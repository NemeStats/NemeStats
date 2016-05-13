using System;
using BusinessLogic.Models.Achievements;

namespace UI.Models.Achievements
{
    public class AchievementViewModel
    {
        public AchievementViewModel(
            string name, 
            string description,
            string fontAwesomeIcon, 
            AchievementLevel achievementLevel,
            int achievementLevel1Threshold,
            int achievementLevel2Threshold,
            int achievementLevel3Threshold)
        {
            Name = name;
            Description = description;
            FontAwesomeIcon = fontAwesomeIcon;
            AchievementLevel = achievementLevel;
            AchievementLevel1Threshold = achievementLevel1Threshold;
            AchievementLevel2Threshold = achievementLevel2Threshold;
            AchievementLevel3Threshold = achievementLevel3Threshold;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string FontAwesomeIcon { get; set; }
        public AchievementLevel AchievementLevel { get; set; }
        public int AchievementLevel1Threshold { get; set; }
        public int AchievementLevel2Threshold { get; set; }
        public int AchievementLevel3Threshold { get; set; }
    }
}