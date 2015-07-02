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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using BusinessLogic.Models.Players;
using UI.Models.PlayedGame;

namespace UI.Models.Players
{
    public class PlayerDetailsViewModel : IEditableViewModel, IGamingGroupAssignedViewModel
    {
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        [Display(Name = "Player Registered")]
        public bool PlayerRegistered { get; set; }
        public bool Active { get; set; }
        public List<GameResultViewModel> PlayerGameResultDetails { get; set; }
        public int TotalGamesPlayed { get; set; }
        public int TotalPoints { get; set; }
        public float AveragePointsPerGame { get; set; }
        public float AveragePlayersPerGame { get; set; }
        public float AveragePointsPerPlayer { get; set; }
        public bool HasNemesis { get; set; }
        public int NemesisPlayerId { get; set; }
        public string NemesisName { get; set; }
        public float LossPercentageVersusPlayer { get; set; }
        public int NumberOfGamesLostVersusNemesis { get; set; }
        public bool UserCanEdit { get; set; }
        public List<MinionViewModel> Minions { get; set; }
        public List<PlayerGameSummaryViewModel> PlayerGameSummaries { get; set; }
        public List<ChampionViewModel> ChampionedGames { get; set; } 

        public string GamingGroupName { get; set; }
        public int GamingGroupId { get; set; }
        public string MinionBraggingTweetUrl { get; set; }
        public PlayerVersusPlayersViewModel PlayerVersusPlayers { get; set; }
        public int TotalGamesWon { get; set; }
        public int TotalGamesLost { get; set; }
        public int WinPercentage { get; set; }
    }
}
