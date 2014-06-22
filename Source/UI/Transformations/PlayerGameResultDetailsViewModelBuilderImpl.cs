using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UI.Models.PlayedGame;

namespace UI.Transformations
{
    public class PlayerGameResultDetailsViewModelBuilderImpl : PlayerGameResultDetailsViewModelBuilder
    {
        internal const string EXCEPTION_PLAYER_CANNOT_BE_NULL = "PlayerGameResult.Player cannot be null.";

        public PlayerGameResultDetailsViewModel Build(PlayerGameResult playerGameResult)
        {
            if (playerGameResult == null)
            {
                throw new ArgumentNullException("playerGameResult");
            }

            if(playerGameResult.Player == null)
            {
                throw new ArgumentException(EXCEPTION_PLAYER_CANNOT_BE_NULL);
            }

            PlayerGameResultDetailsViewModel result = new PlayerGameResultDetailsViewModel();
            result.PlayerId = playerGameResult.PlayerId;
            result.PlayerName = playerGameResult.Player.Name;
            result.GameRank = playerGameResult.GameRank;
            result.GordonPoints = playerGameResult.GordonPoints;

            return result;
        }
    }
}