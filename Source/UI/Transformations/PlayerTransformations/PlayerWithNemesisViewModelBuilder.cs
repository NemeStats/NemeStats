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
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using UI.Models.Players;
using BusinessLogic.Logic.Utilities;
using UI.Models.Points;

namespace UI.Transformations.PlayerTransformations
{
    public class PlayerWithNemesisViewModelBuilder : IPlayerWithNemesisViewModelBuilder
    {
        public PlayerWithNemesisViewModel Build(PlayerWithNemesis playerWithNemesis, string email,
            ApplicationUser currentUser)
        {
            ValidatePlayerNotNull(playerWithNemesis);

            AddInactivePlayerSuffix(playerWithNemesis);

            var totalGamesPlayed = playerWithNemesis.GamesLost + playerWithNemesis.GamesWon;
            var model = new PlayerWithNemesisViewModel
            {
                PlayerId = playerWithNemesis.PlayerId,
                PlayerName = playerWithNemesis.PlayerName,
                PlayerActive = playerWithNemesis.PlayerActive,
                PlayerRegistered = playerWithNemesis.PlayerRegistered,
                RegisteredUserEmailAddress = email,
                UserCanEdit = (currentUser != null && playerWithNemesis.GamingGroupId == currentUser.CurrentGamingGroupId),
                NemesisPlayerId = playerWithNemesis.NemesisPlayerId,
                NemesisPlayerName = playerWithNemesis.NemesisPlayerName,
                PreviousNemesisPlayerId = playerWithNemesis.PreviousNemesisPlayerId,
                PreviousNemesisPlayerName = playerWithNemesis.PreviousNemesisPlayerName,
                NumberOfPlayedGames = totalGamesPlayed,
                OverallWinPercentage = WinPercentageCalculator.CalculateWinPercentage(playerWithNemesis.GamesWon, playerWithNemesis.GamesLost),
                NemePointsSummary = new NemePointsSummaryViewModel(playerWithNemesis.NemePointsSummary),
                TotalChampionedGames = playerWithNemesis.TotalChampionedGames,
                AveragePointsPerGame = totalGamesPlayed > 0 ? (float)playerWithNemesis.NemePointsSummary.TotalPoints / (float)totalGamesPlayed : 0,
                AchievementsPerLevel = playerWithNemesis.AchievementsPerLevel
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
