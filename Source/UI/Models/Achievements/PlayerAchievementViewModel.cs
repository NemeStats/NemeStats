using System;
using System.Collections.Generic;

namespace UI.Models.Achievements
{
    public class PlayerAchievementViewModel : PlayerAchievementSummaryViewModel
    {
        public List<int> RelatedEntities { get; set; }
        public int PlayerProgress { get; set; }
        
    }
}