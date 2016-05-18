using System;
using System.Collections.Generic;
using UI.Models.GameDefinitionModels;

namespace UI.Models.Achievements
{
    public class PlayerAchievementViewModel : PlayerAchievementSummaryViewModel
    {
        public int PlayerProgress { get; set; }
        public List<GameDefinitionSummaryListViewModel> RelatedGameDefintions { get; set; }
    }
}