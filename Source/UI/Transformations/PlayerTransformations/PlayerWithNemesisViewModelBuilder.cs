using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UI.Models.Players;

namespace UI.Transformations.PlayerTransformations
{
    public class PlayerWithNemesisViewModelBuilder : UI.Transformations.PlayerTransformations.IPlayerWithNemesisViewModelBuilder
    {
        internal const string EXCEPTION_MESSAGE_NEMESIS_PLAYER_CANNOT_BE_NULL = "player.Nemesis.NemesisPlayer cannot be null if there is a Nemesis set.";

        public PlayerWithNemesisViewModel Build(Player player)
        {
            ValidatePlayerNotNull(player);

            int? nemesisPlayerId = null;
            string nemesisPlayerName = string.Empty;
            if(player.Nemesis != null)
            {
                ValidateNemesisPlayerNotNullIfNemesisExists(player);

                nemesisPlayerId = player.Nemesis.NemesisPlayerId;
                nemesisPlayerName = player.Nemesis.NemesisPlayer.Name;
            }

            return new PlayerWithNemesisViewModel()
            {
                PlayerId = player.Id,
                PlayerName = player.Name,
                NemesisPlayerId = nemesisPlayerId,
                NemesisPlayerName = nemesisPlayerName
            };
        }

        private static void ValidateNemesisPlayerNotNullIfNemesisExists(Player player)
        {
            if (player.Nemesis.NemesisPlayer == null)
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_NEMESIS_PLAYER_CANNOT_BE_NULL);
            }
        }

        private static void ValidatePlayerNotNull(Player player)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }
        }
    }
}
