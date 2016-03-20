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
using System.Linq;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using UI.Models.Players;
using BusinessLogic.Logic.Utilities;
using UI.Models.Points;

namespace UI.Transformations.PlayerTransformations
{
    public class PlayerWithNemesisViewModelBuilder : UI.Transformations.PlayerTransformations.IPlayerWithNemesisViewModelBuilder
    {
        public PlayerWithNemesisViewModel Build(PlayerWithNemesis playerWithNemesis, ApplicationUser currentUser)
        {
            ValidatePlayerNotNull(playerWithNemesis);

            AddInactivePlayerSuffix(playerWithNemesis);

            var totalGamesPlayed = playerWithNemesis.GamesLost + playerWithNemesis.GamesWon;
            PlayerWithNemesisViewModel model = new PlayerWithNemesisViewModel
            {
                PlayerId = playerWithNemesis.PlayerId,
                PlayerName = playerWithNemesis.PlayerName,
                PlayerActive = playerWithNemesis.PlayerActive,
                PlayerRegistered = playerWithNemesis.PlayerRegistered,
                UserCanEdit = (currentUser != null && playerWithNemesis.GamingGroupId == currentUser.CurrentGamingGroupId),
                NemesisPlayerId = playerWithNemesis.NemesisPlayerId,
                NemesisPlayerName = playerWithNemesis.NemesisPlayerName,
                PreviousNemesisPlayerId = playerWithNemesis.PreviousNemesisPlayerId,
                PreviousNemesisPlayerName = playerWithNemesis.PreviousNemesisPlayerName,
                NumberOfPlayedGames = totalGamesPlayed,
                OverallWinPercentage = WinPercentageCalculator.CalculateWinPercentage(playerWithNemesis.GamesWon, playerWithNemesis.GamesLost),
                //TODO fix this
                NemePointsSummary = new NemePointsSummaryViewModel(0, 0, 0),
                //TotalPoints = playerWithNemesis.TotalPoints,
                TotalChampionedGames = playerWithNemesis.TotalChampionedGames,
                //TODO fix this
                AveragePointsPerGame = totalGamesPlayed > 0 ? (float)playerWithNemesis.TotalPoints / (float)totalGamesPlayed : 0
            };

            return model;
        }

        private static void ValidatePlayerNotNull(PlayerWithNemesis player)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }
        }

        private static void AddInactivePlayerSuffix(PlayerWithNemesis playerWithNemesis)
        {
            if (!playerWithNemesis.PlayerActive)
            {
                playerWithNemesis.PlayerName += " (INACTIVE)";
            }
        }
    }
}
