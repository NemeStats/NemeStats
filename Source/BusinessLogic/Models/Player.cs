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
using System;
using BusinessLogic.DataAccess;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BusinessLogic.Models.User;

namespace BusinessLogic.Models
{
    public class Player : SecuredEntityWithTechnicalKey<int>
    {
        public Player()
        {
            Active = true;
            DateCreated = DateTime.UtcNow;
        }

        public override int Id { get; set; }
        [Index("IX_ID_AND_NAME", 1, IsUnique = true)]
        public override int GamingGroupId { get; set; }

        [StringLength(255)]
        [Index("IX_ID_AND_NAME", 2, IsUnique = true)]
        [Required]
        public string Name { get; set; }
        public string ApplicationUserId { get; set; }
        public bool Active { get; set; }
        public int? NemesisId { get; set; }
        public int? PreviousNemesisId { get; set; }
        public DateTime DateCreated { get; set; }

        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser User { get; set; }
        public virtual GamingGroup GamingGroup { get; set; }
        public virtual Nemesis Nemesis { get; set; }
        public virtual Nemesis PreviousNemesis { get; set; }
        public virtual IList<PlayerGameResult> PlayerGameResults { get; set; }
        public virtual IList<Champion> ChampionedGames { get; set; }
        public virtual IList<PlayerAchievement> PlayerAchievements { get; set; }

        //--records where this player was the Minion
        [InverseProperty("MinionPlayer")]
        public virtual IList<Nemesis> CurrentAndPriorNemeses { get; set; }
        //--records where this player was the Nemesis
        [InverseProperty("NemesisPlayer")]
        public virtual IList<Nemesis> CurrentAndPriorMinions { get; set; }

        public virtual IList<GamingGroupInvitation> Invitations { get; set; }
    }
}
