using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
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
        public AchievementId AchievementId { get; set; }
        public AchievementLevelEnum AchievementLevel { get; set; }

        public virtual Player Player { get; set; }

        [NotMapped]
        public List<int> RelatedEntities
        {
            get
            {
                var entities = this.RelatedEntities_PlainArray.Split(',');
                return entities.Select(int.Parse).ToList();
            }
            set { this.RelatedEntities_PlainArray = string.Join(",", value.Select(p => p.ToString()).ToArray()); }
        }

        public string RelatedEntities_PlainArray { get; set; }

    }
}
