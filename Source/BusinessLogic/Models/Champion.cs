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
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;

namespace BusinessLogic.Models
{
    public class Champion : EntityWithTechnicalKey<int>
    {
        public Champion()
        {
            DateCreated = DateTime.UtcNow;
        }

        public override int Id { get; set; }
        public int GameDefinitionId { get; set; }
        public int PlayerId { get; set; }
        public DateTime DateCreated { get; set; }
        public int NumberOfGames { get; set; }
        public int NumberOfWins { get; set; }
        public float WinPercentage { get; set; }

        [ForeignKey("GameDefinitionId")]
        public virtual GameDefinition GameDefinition { get; set; }

        [ForeignKey("PlayerId")]
        public virtual Player Player { get; set; }

        public bool SameChampion(Champion otherChampion)
        {
            return otherChampion != null 
                && otherChampion.PlayerId == this.PlayerId;
        }

        public override bool Equals(object obj)
        {
            Champion championToCompare = obj as Champion;
            if (championToCompare == null)
            {
                return false;
            }
            return this.GameDefinitionId == championToCompare.GameDefinitionId
                   && this.PlayerId == championToCompare.PlayerId
                   && this.WinPercentage == championToCompare.WinPercentage
                   && this.NumberOfGames == championToCompare.NumberOfGames
                   && this.NumberOfWins == championToCompare.NumberOfWins;
        }
    }
}
