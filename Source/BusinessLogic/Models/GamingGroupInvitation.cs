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
using BusinessLogic.Models.User;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BusinessLogic.Models
{
    public class GamingGroupInvitation : SecuredEntityWithTechnicalKey<Guid>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override Guid Id { get; set; }
        public override int GamingGroupId { get; set; }
        [StringLength(255)]
        public string InviteeEmail { get; set; }
        public string InvitingUserId { get; set; }
        public DateTime DateSent { get; set; }
        public string RegisteredUserId { get; set; }
        public DateTime? DateRegistered { get; set; }
        public int? PlayerId { get; set; }
        [ForeignKey("InvitingUserId")]
        public virtual ApplicationUser InvitingUser { get; set; }
        [ForeignKey("RegisteredUserId")]
        public virtual ApplicationUser RegisteredUser { get; set; }
        [ForeignKey("PlayerId")]
        public virtual Player RegisteredPlayer { get; set; }
    }
}
