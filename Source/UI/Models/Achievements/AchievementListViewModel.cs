using System.Collections.Generic;

namespace UI.Models.Achievements
{
    public class AchievementListViewModel
    {
        public string CurrentUserId { get; set; }
        public List<AchievementViewModel> Achievements { get; set; }
    }
}