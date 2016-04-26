using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using BusinessLogic.DataAccess;

namespace BusinessLogic.Models
{
    public class Achievement : EntityWithTechnicalKey<int>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FontAwesomeIcon { get; set; }
        public int AchievementLevel1Threshold { get; set; }
        public int AchievementLevel2Threshold { get; set; }
        public int AchievementLevel3Threshold { get; set; }
        public string PlayerCalculationSql { get; set; }

        public virtual IList<PlayerAchievement> PlayerAchievements { get; set; }
    }
}
