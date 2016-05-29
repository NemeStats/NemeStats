using System.Collections.Generic;
using BusinessLogic.Models.Achievements;
using UI.Models.Players;

namespace UI.Models.Achievements
{
    public class AchievementViewModel : AchievementSummaryViewModel
    {
        public Dictionary<AchievementLevel, int> LevelThresholds { get; set; }
        public string Description { get; set; }
        public AchievementGroup Group { get; set; }

        public List<PlayerAchievementWinnerViewModel> Winners { get; set; }
    }
}