#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion
using BusinessLogic.DataAccess;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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
            return nemesis != null
                    && this.NemesisPlayerId == nemesis.NemesisPlayerId
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

        public override int GetHashCode()
        {
            return this.MinionPlayerId.GetHashCode() ^ this.NemesisPlayerId ^ this.NumberOfGamesLost ^ (int)this.LossPercentage;
        }
    }
}
