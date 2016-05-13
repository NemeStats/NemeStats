using System.Collections.Generic;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Logic.Achievements
{
    public class AchievementAwarded
    {
        public AchievementAwarded()
        {
            RelatedEntities = new List<int>();
        }

        public AchievementLevel? LevelAwarded { get; set; }
        public AchievementId AchievementId { get; set; }
        public List<int> RelatedEntities { get; set; }
    }
}