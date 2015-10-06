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
using UI.Models.Badges;
using UI.Models.Players;

namespace UI.Transformations.PlayerTransformations
{
    public class PlayerWithNemesisViewModelBuilder : UI.Transformations.PlayerTransformations.IPlayerWithNemesisViewModelBuilder
    {
        public PlayerWithNemesisViewModel Build(PlayerWithNemesis playerWithNemesis, ApplicationUser currentUser)
        {
            ValidatePlayerNotNull(playerWithNemesis);

            PlayerWithNemesisViewModel model = new PlayerWithNemesisViewModel()
            {
                PlayerId = playerWithNemesis.PlayerId,
                PlayerName = playerWithNemesis.PlayerName,
                PlayerRegistered = playerWithNemesis.PlayerRegistered,
                UserCanEdit = (currentUser != null && playerWithNemesis.GamingGroupId == currentUser.CurrentGamingGroupId)
            };

            model.NemesisPlayerId = playerWithNemesis.NemesisPlayerId;
            model.NemesisPlayerName = playerWithNemesis.NemesisPlayerName;

            model.PreviousNemesisPlayerId = playerWithNemesis.PreviousNemesisPlayerId;
            model.PreviousNemesisPlayerName = playerWithNemesis.PreviousNemesisPlayerName;

            model.NumberOfPlayedGames = playerWithNemesis.NumberOfPlayedGames;
            model.TotalPoints = playerWithNemesis.TotalPoints;
            model.ChampionBadges =
                playerWithNemesis.Championships.Select(
                    c => new ChampionBadgeViewModel {GameName = c.GameDefinition.Name}).ToList();

            return model;
        }

        private static void ValidatePlayerNotNull(PlayerWithNemesis player)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }
        }
    }
}
