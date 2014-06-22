using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UI.Models.PlayedGame;

namespace UI.Transformations
{
    public class GameResultViewModelBuilderImpl : GameResultViewModelBuilder
    {
        internal const string EXCEPTION_PLAYER_CANNOT_BE_NULL = "PlayerGameResult.Player cannot be null.";

        public GameResultViewModel Build(int gameDefinitionId, string gameName, PlayerGameResult playerGameResult)
        {
            Validate(playerGameResult);

            GameResultViewModel result = new GameResultViewModel();
            result.PlayerId = playerGameResult.PlayerId;
            result.PlayerName = playerGameResult.Player.Name;
            result.GameRank = playerGameResult.GameRank;
            result.GordonPoints = playerGameResult.GordonPoints;
            result.GameDefinitionId = gameDefinitionId;
            result.GameName = gameName;

            return result;
        }

        private static void Validate(PlayerGameResult playerGameResult)
        {
            if (playerGameResult == null)
            {
                throw new ArgumentNullException("playerGameResult");
            }

            if (playerGameResult.Player == null)
            {
                throw new ArgumentException(EXCEPTION_PLAYER_CANNOT_BE_NULL);
            }
        }
    }
}