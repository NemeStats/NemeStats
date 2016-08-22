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
using BusinessLogic.Models.Games.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BusinessLogic.Models.PlayedGames;
using BusinessLogic.Models.Validation;

namespace BusinessLogic.Models.Games
{
    public class NewlyCompletedGame : ISynchable
    {
        public NewlyCompletedGame()
        {
            DatePlayed = DateTime.UtcNow;
        }

        public int? GameDefinitionId { get; set; }
        public int? BoardGameGeekGameDefinitionId { get; set; }
        public string GameDefinitionName { get; set; }

        public string Notes { get; set; }

        [PlayerRankValidationAttribute]
        [Required]
        public List<PlayerRank> PlayerRanks { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [MaxDate]
        public DateTime DatePlayed { get; set; }

        public WinnerTypes? WinnerType { get; set; }
        public int? GamingGroupId { get; set; }
        public string ExternalSourceApplicationName { get; set; }
        public string ExternalSourceEntityId { get; set; }
    }
}
