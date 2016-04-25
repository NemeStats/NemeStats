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

        public virtual IList<PlayerAchievement> PlayerAchievements { get; set; }
    }
}
