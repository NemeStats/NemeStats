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
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using UI.Models;
using UI.Models.PlayedGame;
using UI.Models.Players;

namespace UI.Transformations.PlayerTransformations
{
    public class PlayerDetailsViewModelBuilder : IPlayerDetailsViewModelBuilder
    {
        internal const string EXCEPTION_PLAYER_GAME_RESULTS_CANNOT_BE_NULL = "PlayerDetails.PlayerGameResults cannot be null.";
        internal const string EXCEPTION_PLAYER_STATISTICS_CANNOT_BE_NULL = "PlayerDetails.PlayerStatistics cannot be null.";
        internal const string EXCEPTION_MINIONS_CANNOT_BE_NULL = "PlayerDetails.Minions cannot be null.";
        internal const string EXCEPTION_CHAMPIONED_GAMES_CANNOT_BE_NULL = "PlayerDetails.ChampionedGames cannot be null.";

        private readonly IGameResultViewModelBuilder gameResultViewModelBuilder;
        private readonly IMinionViewModelBuilder minionViewModelBuilder;
        private readonly IChampionViewModelBuilder championViewModelBuilder;

        public PlayerDetailsViewModelBuilder(IGameResultViewModelBuilder builder, IMinionViewModelBuilder minionViewModelBuilder, IChampionViewModelBuilder championViewModelBuilder)
        {
            gameResultViewModelBuilder = builder;
            this.minionViewModelBuilder = minionViewModelBuilder;
            this.championViewModelBuilder = championViewModelBuilder;
        }

        public PlayerDetailsViewModel Build(PlayerDetails playerDetails, string urlForMinionBragging, ApplicationUser currentUser = null)
        {
            Validate(playerDetails);

            var playerDetailsViewModel = new PlayerDetailsViewModel
            {
                PlayerId = playerDetails.Id,
                PlayerName = playerDetails.Name,
                PlayerRegistered = playerDetails.ApplicationUserId != null,
                Active = playerDetails.Active,
                GamingGroupName = playerDetails.GamingGroupName,
                GamingGroupId = playerDetails.GamingGroupId,
                TotalGamesPlayed = playerDetails.PlayerStats.TotalGames,
                TotalPoints = playerDetails.PlayerStats.TotalPoints
            };

            PopulatePlayerVersusPlayersViewModel(playerDetails, playerDetailsViewModel);

            SetTwitterBraggingUrlIfThePlayerIsTheCurrentlyLoggedInUser(playerDetails, urlForMinionBragging, currentUser, playerDetailsViewModel);
            
            SetAveragePointsPerGame(playerDetails, playerDetailsViewModel);
            playerDetailsViewModel.AveragePlayersPerGame = playerDetails.PlayerStats.AveragePlayersPerGame;
            SetAveragePointsPerPlayer(playerDetails, playerDetailsViewModel);
            SetUserCanEditFlag(playerDetails, currentUser, playerDetailsViewModel);

            PopulatePlayerGameSummaries(playerDetails, playerDetailsViewModel);

            PopulateNemesisData(playerDetails.CurrentNemesis, playerDetailsViewModel);

            playerDetailsViewModel.Minions = (from Player player in playerDetails.Minions
                                              select minionViewModelBuilder.Build(player)).ToList();

            playerDetailsViewModel.PlayerGameSummaries = playerDetails.PlayerGameSummaries;

            SetChampionedGames(playerDetails, playerDetailsViewModel);
            
            return playerDetailsViewModel;
        }

        private static void PopulatePlayerVersusPlayersViewModel(PlayerDetails playerDetails, PlayerDetailsViewModel playerDetailsViewModel)
        {
            var playerVersusPlayers = new PlayerVersusPlayersViewModel();

            foreach (var playerVersusPlayerStatistics in playerDetails.PlayerVersusPlayersStatistics)
            {
                var winPercentage = GetWinPercentage(playerVersusPlayerStatistics);

                playerVersusPlayers.OpposingPlayers.Add(new OpposingPlayerViewModel
                {
                    Name = playerVersusPlayerStatistics.OpposingPlayerName,
                    PlayerId = playerVersusPlayerStatistics.OpposingPlayerId,
                    NumberOfGamesWonVersusThisPlayer = playerVersusPlayerStatistics.NumberOfGamesWonVersusThisPlayer,
                    NumberOfGamesLostVersusThisPlayer = playerVersusPlayerStatistics.NumberOfGamesLostVersusThisPlayer,
                    WinPercentageVersusThisPlayer = (int)winPercentage,
                    IsNemesis = (playerDetails.CurrentNemesis != null 
                        && playerDetails.CurrentNemesis.NemesisPlayerId == playerVersusPlayerStatistics.OpposingPlayerId),
                    IsMinion = playerDetails.Minions.Any(x => x.Id == playerVersusPlayerStatistics.OpposingPlayerId)
                });
            }

            playerDetailsViewModel.PlayerVersusPlayers = playerVersusPlayers;
        }

        private static decimal GetWinPercentage(PlayerVersusPlayerStatistics playerVersusPlayerStatistics)
        {
            decimal winPercentage = 0;

            if (playerVersusPlayerStatistics.NumberOfGamesLostVersusThisPlayer + playerVersusPlayerStatistics.NumberOfGamesWonVersusThisPlayer > 0)
            {
                winPercentage = ((decimal)playerVersusPlayerStatistics.NumberOfGamesWonVersusThisPlayer 
                    / (playerVersusPlayerStatistics.NumberOfGamesWonVersusThisPlayer 
                        + playerVersusPlayerStatistics.NumberOfGamesLostVersusThisPlayer) * 100);
            }
            return winPercentage;
        }

        private static void SetTwitterBraggingUrlIfThePlayerIsTheCurrentlyLoggedInUser(PlayerDetails playerDetails,
                                                                                       string urlForMinionBragging,
                                                                                       ApplicationUser currentUser,
                                                                                       PlayerDetailsViewModel playerDetailsViewModel)
        {
            if (currentUser != null && currentUser.Id == playerDetails.ApplicationUserId)
            {
                playerDetailsViewModel.MinionBraggingTweetUrl = urlForMinionBragging;
            }
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
            playerDetailsViewModel.HasNemesis = !(nemesis is NullNemesis);
            if(playerDetailsViewModel.HasNemesis)
            {
                playerDetailsViewModel.NemesisPlayerId = nemesis.NemesisPlayerId;
                playerDetailsViewModel.NemesisName = nemesis.NemesisPlayer.Name;
                playerDetailsViewModel.NumberOfGamesLostVersusNemesis = nemesis.NumberOfGamesLost;
                playerDetailsViewModel.LossPercentageVersusPlayer = nemesis.LossPercentage;
            }
        }

        private void SetChampionedGames(PlayerDetails playerDetails, PlayerDetailsViewModel playerDetailsViewModel)
        {
            playerDetailsViewModel.ChampionedGames = playerDetails.ChampionedGames.Select(
                championedGame => championViewModelBuilder.Build(championedGame))
                .ToList();
        }

        private static void Validate(PlayerDetails playerDetails)
        {
            ValidatePlayerDetailsIsNotNull(playerDetails);
            ValidatePlayerGameResultsIsNotNull(playerDetails);
            ValidatePlayerStatisticsIsNotNull(playerDetails);
            ValidateMinions(playerDetails);
            ValidateChampionedGames(playerDetails);
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

        private static void ValidateMinions(PlayerDetails playerDetails)
        {
            if (playerDetails.Minions == null)
            {
                throw new ArgumentException(EXCEPTION_MINIONS_CANNOT_BE_NULL);
            }
        }

        private static void ValidateChampionedGames(PlayerDetails playerDetails)
        {
            if (playerDetails.ChampionedGames == null)
            {
                throw new ArgumentException(EXCEPTION_CHAMPIONED_GAMES_CANNOT_BE_NULL);
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