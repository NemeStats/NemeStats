using BusinessLogic.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    public class Player : SecuredEntityWithTechnicalKey<int>
    {
        public Player()
        {
            Active = true;
        }

        public override int Id { get; set; }
        [Index("IX_ID_AND_NAME", 1, IsUnique = true)]
        public override int GamingGroupId { get; set; }

        [StringLength(255)]
        [Index("IX_ID_AND_NAME", 2, IsUnique = true)]
        [Required]
        public string Name { get; set; }
        public bool Active { get; set; }
        public int? NemesisId { get; set; }
        public int? PreviousNemesisId { get; set; }

        public virtual GamingGroup GamingGroup { get; set; }
        public virtual Nemesis Nemesis { get; set; }
        public virtual Nemesis PreviousNemesis { get; set; }
        public virtual IList<PlayerGameResult> PlayerGameResults { get; set; }
    }
}
