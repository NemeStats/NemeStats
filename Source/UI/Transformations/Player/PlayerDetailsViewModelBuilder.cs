using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UI.Models.PlayedGame;
using UI.Models.Players;

namespace UI.Transformations.Player
{
    public class PlayerDetailsViewModelBuilder : IPlayerDetailsViewModelBuilder
    {
        internal const string EXCEPTION_PLAYER_GAME_RESULTS_CANNOT_BE_NULL = "PlayerDetails.PlayerGameResults cannot be null.";
        internal const string EXCEPTION_PLAYER_STATISTICS_CANNOT_BE_NULL = "PlayerDetails.PlayerStatistics cannot be null.";
        internal IGameResultViewModelBuilder gameResultViewModelBuilder;

        public PlayerDetailsViewModelBuilder(IGameResultViewModelBuilder builder)
        {
            gameResultViewModelBuilder = builder;
        }

        public PlayerDetailsViewModel Build(PlayerDetails playerDetails, ApplicationUser currentUser = null)
        {
            Validate(playerDetails);

            PlayerDetailsViewModel playerDetailsViewModel = new PlayerDetailsViewModel();
            playerDetailsViewModel.PlayerId = playerDetails.Id;
            playerDetailsViewModel.PlayerName = playerDetails.Name;
            playerDetailsViewModel.Active = playerDetails.Active;
            playerDetailsViewModel.TotalGamesPlayed = playerDetails.PlayerStats.TotalGames;
            playerDetailsViewModel.TotalPoints = playerDetails.PlayerStats.TotalPoints;
            SetAveragePointsPerGame(playerDetails, playerDetailsViewModel);
            playerDetailsViewModel.AveragePlayersPerGame = playerDetails.PlayerStats.AveragePlayersPerGame;
            SetAveragePointsPerPlayer(playerDetails, playerDetailsViewModel);
            SetUserCanEditFlag(playerDetails, currentUser, playerDetailsViewModel);

            PopulatePlayerGameSummaries(playerDetails, playerDetailsViewModel);

            PopulateNemesisData(playerDetails.Nemesis, playerDetailsViewModel);

            return playerDetailsViewModel;
        }

        private static void SetAveragePointsPerGame(PlayerDetails playerDetails, PlayerDetailsViewModel playerDetailsViewModel)
        {
            if (playerDetails.PlayerStats.TotalGames == 0)
            {
                playerDetailsViewModel.AveragePointsPerGame = 0;
            }
            else
            {
                playerDetailsViewModel.AveragePointsPerGame = (float)playerDetails.PlayerStats.TotalPoints / (float)playerDetails.PlayerStats.TotalGames;
            }
        }

        private static void SetAveragePointsPerPlayer(PlayerDetails playerDetails, PlayerDetailsViewModel playerDetailsViewModel)
        {
            if (playerDetails.PlayerStats.AveragePlayersPerGame == 0)
            {
                playerDetailsViewModel.AveragePointsPerPlayer = 0;
            }
            else
            {
                playerDetailsViewModel.AveragePointsPerPlayer
                    = playerDetailsViewModel.AveragePointsPerGame / playerDetails.PlayerStats.AveragePlayersPerGame;
            }
        }

        private static void SetUserCanEditFlag(PlayerDetails playerDetails, ApplicationUser currentUser, PlayerDetailsViewModel playerDetailsViewModel)
        {
            if (currentUser == null || playerDetails.GamingGroupId != currentUser.CurrentGamingGroupId)
            {
                playerDetailsViewModel.UserCanEdit = false;
            }
            else
            {
                playerDetailsViewModel.UserCanEdit = true;
            }
        }

        private static void PopulateNemesisData(Nemesis nemesis, PlayerDetailsViewModel playerDetailsViewModel)
        {
            //TODO should there be another 'normal' type that extends Nemesis?
            playerDetailsViewModel.HasNemesis = !(nemesis is NullNemesis);
            playerDetailsViewModel.NemesisPlayerId = nemesis.NemesisPlayerId;
            playerDetailsViewModel.NemesisName = nemesis.NemesisPlayerName;
            playerDetailsViewModel.NumberOfGamesLostVersusNemesis = nemesis.GamesLostVersusNemesis;
            playerDetailsViewModel.LossPercentageVersusPlayer = nemesis.LossPercentageVersusNemesis;
        }

        private static void Validate(PlayerDetails playerDetails)
        {
            ValidatePlayerDetailsIsNotNull(playerDetails);
            ValidatePlayerGameResultsIsNotNull(playerDetails);
            ValidatePlayerStatisticsIsNotNull(playerDetails);
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

        private static void ValidatePlayerStatisticsIsNotNull(PlayerDetails playerDetails)
        {
            if (playerDetails.PlayerStats == null)
            {
                throw new ArgumentException(EXCEPTION_PLAYER_STATISTICS_CANNOT_BE_NULL);
            }
        }

        private void PopulatePlayerGameSummaries(PlayerDetails playerDetails, PlayerDetailsViewModel playerDetailsViewModel)
        {
            playerDetailsViewModel.PlayerGameResultDetails = new List<GameResultViewModel>();
            GameResultViewModel gameResultViewModel;
            foreach (PlayerGameResult playerGameResult in playerDetails.PlayerGameResults)
            {
                gameResultViewModel = gameResultViewModelBuilder.Build(playerGameResult);
                playerDetailsViewModel.PlayerGameResultDetails.Add(gameResultViewModel);
            }
        }
    }
}