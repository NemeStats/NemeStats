using System.Collections.Generic;

namespace UI.Models.Achievements
{
    public class AchievementListViewModel
    {
        public string CurrentUserId { get; set; }
        public List<AchievementTileViewModel> Achievements { get; set; }
    }
}