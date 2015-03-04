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
using System.ComponentModel.DataAnnotations.Schema;
using BusinessLogic.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Models.User;

namespace BusinessLogic.Models
{
    public class PlayedGame : SecuredEntityWithTechnicalKey<int>
    {
        public PlayedGame()
        {
            DateCreated = DateTime.UtcNow;
        }

        public override int Id { get; set; }

        public override int GamingGroupId { get; set; }
        public int GameDefinitionId { get; set; }
        public int NumberOfPlayers { get; set; }
        public DateTime DatePlayed { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedByApplicationUserId { get; set; }
        public string Notes { get; set; }

        public virtual GamingGroup GamingGroup { get; set; }
        public virtual GameDefinition GameDefinition { get; set; }
        [ForeignKey("CreatedByApplicationUserId")]
        public virtual ApplicationUser CreatedByApplicationUser { get; set; }
        public virtual IList<PlayerGameResult> PlayerGameResults { get; set; }
    }
}
