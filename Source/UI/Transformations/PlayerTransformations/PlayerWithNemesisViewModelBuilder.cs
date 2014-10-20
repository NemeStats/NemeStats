using BusinessLogic.Models;
using System;
using System.Linq;
using UI.Models.Players;

namespace UI.Transformations.PlayerTransformations
{
    public class PlayerWithNemesisViewModelBuilder : UI.Transformations.PlayerTransformations.IPlayerWithNemesisViewModelBuilder
    {
        internal const string EXCEPTION_MESSAGE_NEMESIS_PLAYER_CANNOT_BE_NULL = "player.Nemesis.NemesisPlayer cannot be null if there is a Nemesis set.";
        internal const string EXCEPTION_MESSAGE_PREVIOUS_NEMESIS_PLAYER_CANNOT_BE_NULL = "player.PreviousNemesis.NemesisPlayer cannot be null if there is a PreviousNemesis set.";

        public PlayerWithNemesisViewModel Build(Player player)
        {
            ValidatePlayerNotNull(player);

            PlayerWithNemesisViewModel model = new PlayerWithNemesisViewModel()
            {
                PlayerId = player.Id,
                PlayerName = player.Name
            };

            if(player.Nemesis != null)
            {
                ValidateNemesisPlayerNotNullIfNemesisExists(player);

                model.NemesisPlayerId = player.Nemesis.NemesisPlayerId;
                model.NemesisPlayerName = player.Nemesis.NemesisPlayer.Name;
            }

            if (player.PreviousNemesis != null)
            {
                ValidatePreviousNemesisPlayerNotNullIfNemesisExists(player);

                model.PreviousNemesisPlayerId = player.PreviousNemesis.NemesisPlayerId;
                model.PreviousNemesisPlayerName = player.PreviousNemesis.NemesisPlayer.Name;
            }

            return model;
        }

        private static void ValidatePlayerNotNull(Player player)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }
        }

        private static void ValidateNemesisPlayerNotNullIfNemesisExists(Player player)
        {
            if (player.Nemesis.NemesisPlayer == null)
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_NEMESIS_PLAYER_CANNOT_BE_NULL);
            }
        }

        private static void ValidatePreviousNemesisPlayerNotNullIfNemesisExists(Player player)
        {
            if (player.PreviousNemesis.NemesisPlayer == null)
            {
                throw new ArgumentException(EXCEPTION_MESSAGE_PREVIOUS_NEMESIS_PLAYER_CANNOT_BE_NULL);
            }
        }
    }
}
