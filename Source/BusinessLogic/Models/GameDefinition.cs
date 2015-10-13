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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BusinessLogic.DataAccess;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic.Models
{
    public class GameDefinition : SecuredEntityWithTechnicalKey<int>
    {
        public GameDefinition()
        {
            Active = true;
            DateCreated = DateTime.UtcNow;
        }

        public override int Id { get; set; }

        [Index("IX_ID_AND_NAME", 1, IsUnique = true)]
        public override int GamingGroupId { get; set; }

        public int? BoardGameGeekObjectId { get; set; }
        public string ThumbnailImageUrl { get; set; }

        [StringLength(255)]
        [Index("IX_ID_AND_NAME", 2, IsUnique = true)]
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public DateTime DateCreated { get; set; }

        public virtual IList<PlayedGame> PlayedGames { get; set; }
        public virtual GamingGroup GamingGroup { get; set; }

        public int? ChampionId { get; set; }
        public int? PreviousChampionId { get; set; }

        public virtual Champion Champion { get; set; }
        public virtual Champion PreviousChampion { get; set; }
    }
}
