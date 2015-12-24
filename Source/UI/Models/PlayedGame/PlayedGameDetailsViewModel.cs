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
using BusinessLogic.Models.PlayedGames;

namespace UI.Models.PlayedGame
{
    public class PlayedGameDetailsViewModel : IEditableViewModel, IGamingGroupAssignedViewModel
    {
        public int PlayedGameId { get; set; }
        public string GameDefinitionName { get; set; }
        public int GameDefinitionId { get; set; }
        public int GamingGroupId { get; set; }
        public string GamingGroupName { get; set; }
        public string Notes { get; set; }
        public DateTime DatePlayed { get; set; }
        public IList<GameResultViewModel> PlayerResults { get; set; }

        public GameResultViewModel WinningPlayer
        {
            get
            {
                return WinnerType == WinnerTypes.PlayerWin ? PlayerResults.First(r=>r.GameRank == 1) : null;
            }
        }

        public bool UserCanEdit { get; set; }
        public WinnerTypes? WinnerType { get; set; }

        public Uri BoardGameGeekUri { get; set; }
        public string ThumbnailImageUrl { get; set; }
    }
}