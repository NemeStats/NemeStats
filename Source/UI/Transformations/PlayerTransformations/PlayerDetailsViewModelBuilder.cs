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

#endregion LICENSE

using AutoMapper;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Logic;
using BusinessLogic.Logic.Players;
using UI.Models.Achievements;
using UI.Models.Badges;
using UI.Models.PlayedGame;
using UI.Models.Players;
using UI.Models.Points;

namespace UI.Transformations.PlayerTransformations
{
    public class PlayerDetailsViewModelBuilder : IPlayerDetailsViewModelBuilder
    {
        internal const string EXCEPTION_PLAYER_GAME_RESULTS_CANNOT_BE_NULL = "PlayerDetails.PlayerGameResults cannot be null.";
        internal const string EXCEPTION_PLAYER_STATISTICS_CANNOT_BE_NULL = "PlayerDetails.PlayerStatistics cannot be null.";
        internal const string EXCEPTION_MINIONS_CANNOT_BE_NULL = "PlayerDetails.Minions cannot be null.";
        internal const string EXCEPTION_CHAMPIONED_GAMES_CANNOT_BE_NULL = "PlayerDetails.ChampionedGames cannot be null.";
        internal const string EXCEPTION_FORMERCHAMPIONED_GAMES_CANNOT_BE_NULL = "PlayerDetails.FormerChampionedGames cannot be null.";

        private readonly IGameResultViewModelBuilder _gameResultViewModelBuilder;
        private readonly IMinionViewModelBuilder _minionViewModelBuilder;
        private readonly ITransformer _transformer;


        public PlayerDetailsViewModelBuilder(
            IGameResultViewModelBuilder builder,
            IMinionViewModelBuilder minionViewModelBuilder, 
            ITransformer transformer)
        {
            _gameResultViewModelBuilder = builder;
            _minionViewModelBuilder = minionViewModelBuilder;
            _transformer = transformer;
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
                NemePointsSummary = new NemePointsSummaryViewModel(playerDetails.NemePointsSummary),
                TotalGamesWon = playerDetails.PlayerStats.TotalGamesWon,
                TotalGamesLost = playerDetails.PlayerStats.TotalGamesLost,
                WinPercentage = playerDetails.PlayerStats.WinPercentage,
                TotalChampionedGames = playerDetails.ChampionedGames.Count,
                LongestWinningStreak = playerDetails.LongestWinningStreak,
                PlayerAchievements = playerDetails.Achievements.Select(x => _transformer.Transform<PlayerAchievementSummaryViewModel>(x))
                .OrderByDescending(a => a.AchievementLevel)
                                                  .ThenByDescending(a => a.LastUpdatedDate)
                                                  .ToList()
            };

            PopulatePlayerVersusPlayersViewModel(playerDetails, playerDetailsViewModel);

            SetTwitterBraggingUrlIfThePlayerIsTheCurrentlyLoggedInUser(playerDetails, urlForMinionBragging, currentUser, playerDetailsViewModel);

            SetAveragePointsPerGame(playerDetails, playerDetailsViewModel);
            playerDetailsViewModel.AveragePlayersPerGame = playerDetails.PlayerStats.AveragePlayersPerGame;
            SetAveragePointsPerPlayer(playerDetails, playerDetailsViewModel);
            SetUserCanEditFlag(playerDetails, currentUser, playerDetailsViewModel);

            PopulatePlayerGameResults(playerDetails, playerDetailsViewModel);

            PopulateNemesisData(playerDetails.CurrentNemesis, playerDetailsViewModel);

            playerDetailsViewModel.Minions = (from Player player in playerDetails.Minions
                                              select _minionViewModelBuilder.Build(player)).ToList();

            playerDetailsViewModel.PlayerGameSummaries = playerDetails.PlayerGameSummaries.Select(Mapper.Map<PlayerGameSummaryViewModel>).ToList();

            SetChampionedGames(playerDetails, playerDetailsViewModel);

            SetFormerChampionedGames(playerDetails, playerDetailsViewModel);

            return playerDetailsViewModel;
        }

        private static void PopulatePlayerVersusPlayersViewModel(PlayerDetails playerDetails, PlayerDetailsViewModel playerDetailsViewModel)
        {
            var playerVersusPlayers = new PlayersSummaryViewModel
            {
                WinLossHeader = "Win - Loss Record vs. Player"
            };

            foreach (var playerVersusPlayerStatistics in playerDetails.PlayerVersusPlayersStatistics)
            {
                var winPercentage = GetWinPercentage(playerVersusPlayerStatistics);

                var playerSummaryViewModel = new PlayerSummaryViewModel
                {
                    PlayerName = PlayerNameBuilder.BuildPlayerName(playerVersusPlayerStatistics.OpposingPlayerName, playerVersusPlayerStatistics.OpposingPlayerActive),
                    PlayerId = playerVersusPlayerStatistics.OpposingPlayerId,
                    GamesWon = playerVersusPlayerStatistics.NumberOfGamesWonVersusThisPlayer,
                    GamesLost = playerVersusPlayerStatistics.NumberOfGamesLostVersusThisPlayer,
                    WinPercentage = (int)winPercentage
                };

                if (playerDetails.CurrentNemesis != null
                    && playerDetails.CurrentNemesis.NemesisPlayerId == playerVersusPlayerStatistics.OpposingPlayerId)
                {
                    playerSummaryViewModel.SpecialBadgeTypes.Add(new NemesisBadgeViewModel());
                }

                if (playerDetails.PreviousNemesis?.NemesisPlayerId == playerVersusPlayerStatistics.OpposingPlayerId)
                {
                    playerSummaryViewModel.SpecialBadgeTypes.Add(new PreviousNemesisBadgeViewModel());
                }

                if (playerDetails.Minions.Any(x => x.Id == playerVersusPlayerStatistics.OpposingPlayerId))
                {
                    playerSummaryViewModel.SpecialBadgeTypes.Add(new MinionBadgeViewModel());
                }

                playerVersusPlayers.PlayerSummaries.Add(playerSummaryViewModel);
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
                playerDetailsViewModel.AveragePointsPerGame = (float)playerDetails.PlayerStats.NemePointsSummary.TotalPoints / (float)playerDetails.PlayerStats.TotalGames;
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
            if (playerDetailsViewModel.HasNemesis)
            {
                playerDetailsViewModel.NemesisPlayerId = nemesis.NemesisPlayerId;
                playerDetailsViewModel.NemesisName = nemesis.NemesisPlayer.Name;
                playerDetailsViewModel.NumberOfGamesLostVersusNemesis = nemesis.NumberOfGamesLost;
                playerDetailsViewModel.LossPercentageVersusPlayer = nemesis.LossPercentage;
            }
        }

        private void SetChampionedGames(PlayerDetails playerDetails, PlayerDetailsViewModel playerDetailsViewModel)
        {
            if (playerDetails.PlayerGameSummaries == null)
            {
                return;
            }
            playerDetailsViewModel.PlayerGameSummaries
                .Where(summary => playerDetails.ChampionedGames
                                    .Select(championedGame => championedGame.GameDefinitionId)
                                    .Contains(summary.GameDefinitionId))
                .ToList()
                .ForEach(x => x.IsChampion = true);
        }

        private void SetFormerChampionedGames(PlayerDetails playerDetails, PlayerDetailsViewModel playerDetailsViewModel)
        {
            if (playerDetails.PlayerGameSummaries == null)
            {
                return;
            }
            playerDetailsViewModel.PlayerGameSummaries
                .Where(summary => playerDetails.FormerChampionedGames.Select(fcg => fcg.Id).Contains(summary.GameDefinitionId)
                    //take the current champion out of the former champions list
                    && !summary.IsChampion)
                .ToList()
                .ForEach(x => x.IsFormerChampion = true);
        }

        private static void Validate(PlayerDetails playerDetails)
        {
            ValidatePlayerDetailsIsNotNull(playerDetails);
            ValidatePlayerGameResultsIsNotNull(playerDetails);
            ValidatePlayerStatisticsIsNotNull(playerDetails);
            ValidateMinions(playerDetails);
            ValidateChampionedGames(playerDetails);
            ValidateFormerChampionedGames(playerDetails);
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

        private static void ValidateFormerChampionedGames(PlayerDetails playerDetails)
        {
            if (playerDetails.FormerChampionedGames == null)
            {
                throw new ArgumentException(EXCEPTION_FORMERCHAMPIONED_GAMES_CANNOT_BE_NULL);
            }
        }

        private void PopulatePlayerGameResults(PlayerDetails playerDetails, PlayerDetailsViewModel playerDetailsViewModel)
        {
            playerDetailsViewModel.PlayerGameResultDetails = new List<GameResultViewModel>();
            GameResultViewModel gameResultViewModel;
            foreach (PlayerGameResult playerGameResult in playerDetails.PlayerGameResults)
            {
                gameResultViewModel = _gameResultViewModelBuilder.Build(playerGameResult);
                playerDetailsViewModel.PlayerGameResultDetails.Add(gameResultViewModel);
            }
        }
    }
}