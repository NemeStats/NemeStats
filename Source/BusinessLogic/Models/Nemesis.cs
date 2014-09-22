using BusinessLogic.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Models
{
    public class Nemesis : EntityWithTechnicalKey<int>
    {
        public Nemesis()
        {
            DateCreated = DateTime.UtcNow;
        }

        public override int Id { get; set; }
        public int MinionPlayerId { get; set; }
        public int NemesisPlayerId { get; set; }
        public DateTime DateCreated { get; set; }
        public int NumberOfGamesLost { get; set; }
        public float LossPercentage { get; set; }

        [ForeignKey("MinionPlayerId")]
        public virtual Player MinionPlayer { get; set; }
        [ForeignKey("NemesisPlayerId")]
        public virtual Player NemesisPlayer { get; set; }
    }
}
