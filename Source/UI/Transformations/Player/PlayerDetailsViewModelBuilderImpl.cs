using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UI.Models.PlayedGame;
using UI.Models.Players;

namespace UI.Transformations.Player
{
    public class PlayerDetailsViewModelBuilderImpl : PlayerDetailsViewModelBuilder
    {
        internal const string EXCEPTION_PLAYER_GAME_RESULTS_CANNOT_BE_NULL = "PlayerGameResults cannot be null.";

        internal IndividualPlayerGameSummaryViewModelBuilder playerGameSummaryBuilder;

        //TODO is this correct? MVC complained that I didn't have a parameterless constructor.
        public PlayerDetailsViewModelBuilderImpl()
        {
            playerGameSummaryBuilder = new IndividualPlayerGameSummaryViewModelBuilderImpl();
        }

        public PlayerDetailsViewModelBuilderImpl(IndividualPlayerGameSummaryViewModelBuilder builder)
        {
            playerGameSummaryBuilder = builder;
        }

        public PlayerDetailsViewModel Build(PlayerDetails playerDetails)
        {
            Validate(playerDetails);

            PlayerDetailsViewModel playerDetailsViewModel = new PlayerDetailsViewModel();
            playerDetailsViewModel.PlayerId = playerDetails.Id;
            playerDetailsViewModel.PlayerName = playerDetails.Name;
            playerDetailsViewModel.Active = playerDetails.Active;

            PopulatePlayerGameSummaries(playerDetails, playerDetailsViewModel);

            return playerDetailsViewModel;
        }

        private static void Validate(PlayerDetails playerDetails)
        {
            ValidatePlayerDetailsIsNotNull(playerDetails);
            ValidatePlayerGameResultsIsNotNull(playerDetails);
        }

        private static void ValidatePlayerDetailsIsNotNull(PlayerDetails playerDetails)
        {
            if (playerDetails == null)
            {
                throw new ArgumentNullException("playerDetails");
            }
        }

        private static void ValidatePlayerGameResultsIsNotNull(PlayerDetails playerDetails)
        {
            if (playerDetails.PlayerGameResults == null)
            {
                throw new ArgumentException(EXCEPTION_PLAYER_GAME_RESULTS_CANNOT_BE_NULL);
            }
        }

        private void PopulatePlayerGameSummaries(PlayerDetails playerDetails, PlayerDetailsViewModel playerDetailsViewModel)
        {
            playerDetailsViewModel.PlayerGameSummaries = new List<IndividualPlayerGameSummaryViewModel>();
            foreach (PlayerGameResult playerGameResult in playerDetails.PlayerGameResults)
            {
                playerDetailsViewModel.PlayerGameSummaries.Add(playerGameSummaryBuilder.Build(playerGameResult));
            }
        }
    }
}