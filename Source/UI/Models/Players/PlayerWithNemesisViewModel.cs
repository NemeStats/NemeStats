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

using System.Collections.Generic;
using BusinessLogic.Models.Achievements;
using UI.Models.Points;

namespace UI.Models.Players
{
    public class PlayerWithNemesisViewModel : IEditableViewModel
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public bool PlayerActive { get; set; }
        public bool PlayerRegistered { get; set; }
        public int? NemesisPlayerId { get; set; }
        public string NemesisPlayerName { get; set; }
        public int? PreviousNemesisPlayerId { get; set; }
        public string PreviousNemesisPlayerName { get; set; }
        public bool UserCanEdit { get; set; }
        public int NumberOfPlayedGames { get; set; }
        public int OverallWinPercentage { get; set; }
        public NemePointsSummaryViewModel NemePointsSummary { get; set; }
        public int TotalChampionedGames { get; set; }
        public float AveragePointsPerGame { get; set; }
        public Dictionary<AchievementLevel, int> AchievementsPerLevel { get; set; }
        public string RegisteredUserEmailAddress { get; set; }
    }
}
