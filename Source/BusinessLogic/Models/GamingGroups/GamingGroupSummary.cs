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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;

namespace BusinessLogic.Models.GamingGroups
{
    public class GamingGroupSummary
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string OwningUserId { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual ApplicationUser OwningUser { get; set; }
        public virtual IList<PlayerWithNemesis> Players { get; set; }
        public virtual IList<GameDefinitionSummary> GameDefinitionSummaries { get; set; }
        public virtual IList<PlayedGame> PlayedGames { get; set; }
        public virtual IList<GamingGroupInvitation> GamingGroupInvitations { get; set; }
    }
}
