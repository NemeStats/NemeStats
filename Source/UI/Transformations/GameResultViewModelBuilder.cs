using BusinessLogic.Models;
using System;
using System.Linq;
using UI.Models.PlayedGame;

namespace UI.Transformations
{
    public class GameResultViewModelBuilder : IGameResultViewModelBuilder
    {
        internal const string EXCEPTION_PLAYER_CANNOT_BE_NULL = "PlayerGameResult.Player cannot be null.";
        internal const string EXCEPTION_GAME_DEFINITION_CANNOT_BE_NULL = "PlayerGameResult.PlayedGame.GameDefinition cannot be null.";
        internal const string EXCEPTION_PLAYED_GAME_CANNOT_BE_NULL = "PlayerGameResult.PlayedGame cannot be null.";

        public GameResultViewModel Build(PlayerGameResult playerGameResult)
        {
            Validate(playerGameResult);

            GameResultViewModel result = new GameResultViewModel();
            result.PlayerId = playerGameResult.PlayerId;
            result.PlayerName = playerGameResult.Player.Name;
            result.GameRank = playerGameResult.GameRank;
            result.GordonPoints = playerGameResult.GordonPoints;
            result.GameDefinitionId = playerGameResult.PlayedGame.GameDefinition.Id;
            result.GameName = playerGameResult.PlayedGame.GameDefinition.Name;
            result.PlayedGameId = playerGameResult.PlayedGameId;
            result.DatePlayed = playerGameResult.PlayedGame.DatePlayed;

            return result;
        }

        private static void Validate(PlayerGameResult playerGameResult)
        {
            ValidatePlayerGameResultIsNotNull(playerGameResult);
            ValidatePlayerIsNotNull(playerGameResult);
            ValidatePlayedGameIsNotNull(playerGameResult);
            ValidateGameDefinitionIsNotNull(playerGameResult);
        }

        private static void ValidatePlayerGameResultIsNotNull(PlayerGameResult playerGameResult)
        {
            if (playerGameResult == null)
            {
                throw new ArgumentNullException("playerGameResult");
            }
        }

        private static void ValidatePlayerIsNotNull(PlayerGameResult playerGameResult)
        {
            if (playerGameResult.Player == null)
            {
                throw new ArgumentException(EXCEPTION_PLAYER_CANNOT_BE_NULL);
            }
        }

        private static void ValidatePlayedGameIsNotNull(PlayerGameResult playerGameResult)
        {
            if (playerGameResult.PlayedGame == null)
            {
                throw new ArgumentException(EXCEPTION_PLAYED_GAME_CANNOT_BE_NULL);
            }
        }

        private static void ValidateGameDefinitionIsNotNull(PlayerGameResult playerGameResult)
        {
            if (playerGameResult.PlayedGame.GameDefinition == null)
            {
                throw new ArgumentException(EXCEPTION_GAME_DEFINITION_CANNOT_BE_NULL);
            }
        }
    }
}