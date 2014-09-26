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

        /// <summary>
        /// Indicates whether the two records represent the same Minion to Nemesis relationship (regardless of the stats recorded at that time)
        /// </summary>
        public bool SameNemesis(Nemesis nemesis)
        {
            return this.NemesisPlayerId == nemesis.NemesisPlayerId
                    && this.MinionPlayerId == nemesis.MinionPlayerId;
        }

        public override bool Equals(object nemesis)
        {
            Nemesis compareToNemesis = nemesis as Nemesis;
            if(compareToNemesis == null)
            {
                return false;
            }
            return this.MinionPlayerId == compareToNemesis.MinionPlayerId
                    && this.NemesisPlayerId == compareToNemesis.NemesisPlayerId
                    && this.NumberOfGamesLost == compareToNemesis.NumberOfGamesLost
                    && this.LossPercentage == compareToNemesis.LossPercentage;
        }
    }
}
