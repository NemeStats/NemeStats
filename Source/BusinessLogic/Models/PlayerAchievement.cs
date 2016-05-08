using System;
using System.ComponentModel.DataAnnotations.Schema;
using BusinessLogic.DataAccess;
using BusinessLogic.Models.Achievements;

namespace BusinessLogic.Models
{
    public class PlayerAchievement :  EntityWithTechnicalKey<int>
    {
        public PlayerAchievement()
        {
            DateCreated = DateTime.UtcNow;
            LastUpdatedDate = DateTime.UtcNow;
        }

        public override int Id { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        [Index("IX_PLAYERID_AND_ACHIEVEMENTID", 1, IsUnique = true)]
        public int PlayerId { get; set; }
        [Index("IX_PLAYERID_AND_ACHIEVEMENTID", 2, IsUnique = true)]
        public AchievementType AchievementId { get; set; }
        public AchievementLevelEnum AchievementLevel { get; set; }

        public virtual Player Player { get; set; }

    }
}
