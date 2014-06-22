using BusinessLogic.Models;
using System;
using UI.Models.PlayedGame;

namespace UI.Transformations.Player
{
    public class IndividualPlayerGameSummaryViewModelBuilderImpl : IndividualPlayerGameSummaryViewModelBuilder
    {
        internal const string EXCEPTION_PLAYED_GAME_CANNOT_BE_NULL = "PlayedGame cannot be null.";
        internal const string EXCEPTION_GAME_DEFINITION_CANNOT_BE_NULL = "GameDefinition cannot be null.";

        public IndividualPlayerGameSummaryViewModel Build(PlayerGameResult playerGameResult)
        {
            Validate(playerGameResult);

            IndividualPlayerGameSummaryViewModel viewModel = new IndividualPlayerGameSummaryViewModel();
            viewModel.GameRank = playerGameResult.GameRank;
            viewModel.GordonPoints = playerGameResult.GordonPoints;
            viewModel.PlayedGameId = playerGameResult.PlayedGameId;
            viewModel.GameName = playerGameResult.PlayedGame.GameDefinition.Name;

            return viewModel;
        }

        private static void Validate(PlayerGameResult playerGameResult)
        {
            ValidatePlayerGameResultIsNotNull(playerGameResult);
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