using BusinessLogic.Models;
using System;
using System.Linq;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
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
