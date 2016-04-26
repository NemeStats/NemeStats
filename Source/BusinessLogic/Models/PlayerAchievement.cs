using System;
using System.ComponentModel.DataAnnotations.Schema;
using BusinessLogic.DataAccess;

namespace BusinessLogic.Models
{
    public class PlayerAchievement :  EntityWithTechnicalKey<int>
    {
        public PlayerAchievement()
        {
            DateCreated = DateTime.UtcNow;
        }

        public override int Id { get; set; }

        public DateTime DateCreated { get; set; }
        public string Notes { get; set; }
        public int? PlayedGameId { get; set; }
        [Index("IX_PLAYERID_AND_ACHIEVEMENTID", 1, IsUnique = true)]
        public int PlayerId { get; set; }
        [Index("IX_PLAYERID_AND_ACHIEVEMENTID", 2, IsUnique = true)]
        public int AchievementId { get; set; }
        public int AchievementLevel { get; set; }

        public virtual PlayedGame PlayedGameThatTriggeredAchievement { get; set; }
        public virtual Player Player { get; set; }
        public virtual Achievement Achievement { get; set; }
    }
}
