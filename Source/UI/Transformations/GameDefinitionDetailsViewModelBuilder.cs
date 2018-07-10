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

using System.Collections.Generic;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using System.Linq;
using BusinessLogic.Logic;
using BusinessLogic.Logic.Players;
using UI.Models.GameDefinitionModels;
using UI.Models.PlayedGame;
using UI.Models.Players;
using UI.Models.UniversalGameModels;

namespace UI.Transformations
{
    public class GameDefinitionDetailsViewModelBuilder : IGameDefinitionDetailsViewModelBuilder
    {
        private readonly IPlayedGameDetailsViewModelBuilder _playedGameDetailsViewModelBuilder;
        private readonly ITransformer _transformer;

        public GameDefinitionDetailsViewModelBuilder(IPlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilder, ITransformer transformer)
        {
            _playedGameDetailsViewModelBuilder = playedGameDetailsViewModelBuilder;
            _transformer = transformer;
        }

        public GameDefinitionDetailsViewModel Build(
            GameDefinitionSummary gameDefinitionSummary, 
            Dictionary<int, string> playerIdToRegisteredUserEmailDictionary,
            ApplicationUser currentUser)
        {
            BoardGameGeekInfoViewModel boardGameGeekInfoViewModel = null;
            if (gameDefinitionSummary.BoardGameGeekGameDefinitionId.HasValue)
            {
                boardGameGeekInfoViewModel = _transformer.Transform<BoardGameGeekInfoViewModel>(gameDefinitionSummary.BoardGameGeekInfo);
            }
            var viewModel = new GameDefinitionDetailsViewModel
            {
                GameDefinitionId = gameDefinitionSummary.Id,
                GameDefinitionName = gameDefinitionSummary.Name,
                TotalNumberOfGamesPlayed = gameDefinitionSummary.TotalNumberOfGamesPlayed,
                AveragePlayersPerGame = $"{gameDefinitionSummary.AveragePlayersPerGame:0.#}",
                GamingGroupId = gameDefinitionSummary.GamingGroupId,
                GamingGroupName = gameDefinitionSummary.GamingGroupName,
                UserCanEdit = (currentUser != null && gameDefinitionSummary.GamingGroupId == currentUser.CurrentGamingGroupId),
                BoardGameGeekInfo = boardGameGeekInfoViewModel
            };

            SetPlayedGames(gameDefinitionSummary, currentUser, viewModel);

            SetChampionStuff(gameDefinitionSummary, viewModel);

            SetPreviousChampionStuff(gameDefinitionSummary, viewModel);

            SetPlayersStuff(gameDefinitionSummary, playerIdToRegisteredUserEmailDictionary, viewModel);

            return viewModel;
        }

        private void SetPlayersStuff(GameDefinitionSummary gameDefinitionSummary,
            Dictionary<int, string> playerIdToRegisteredUserEmailDictionary, GameDefinitionDetailsViewModel viewModel)
        {
            var gameDefinitionPlayersSummary = new List<GameDefinitionPlayerSummaryViewModel>();
            foreach (var playerWinRecord in gameDefinitionSummary.PlayerWinRecords)
            {
                var transformedResult = _transformer.Transform<GameDefinitionPlayerSummaryViewModel>(playerWinRecord);
                if (playerIdToRegisteredUserEmailDictionary.ContainsKey(playerWinRecord.PlayerId))
                {
                    transformedResult.RegisteredUserEmailAddress =
                        playerIdToRegisteredUserEmailDictionary[playerWinRecord.PlayerId];
                }

                gameDefinitionPlayersSummary.Add(transformedResult);
            }

            viewModel.GameDefinitionPlayersSummary = gameDefinitionPlayersSummary;
        }

        private static void SetPreviousChampionStuff(GameDefinitionSummary gameDefinitionSummary,
            GameDefinitionDetailsViewModel viewModel)
        {
            if (!(gameDefinitionSummary.PreviousChampion is NullChampion))
            {
                viewModel.PreviousChampionName = PlayerNameBuilder.BuildPlayerName(
                    gameDefinitionSummary.PreviousChampion.Player.Name,
                    gameDefinitionSummary.PreviousChampion.Player.Active);
                viewModel.PreviousChampionPlayerId = gameDefinitionSummary.PreviousChampion.Player.Id;
            }
        }

        private static void SetChampionStuff(GameDefinitionSummary gameDefinitionSummary,
            GameDefinitionDetailsViewModel viewModel)
        {
            if (!(gameDefinitionSummary.Champion is NullChampion))
            {
                viewModel.ChampionName = PlayerNameBuilder.BuildPlayerName(
                    gameDefinitionSummary.Champion.Player.Name,
                    gameDefinitionSummary.Champion.Player.Active);
                viewModel.ChampionPlayerId = gameDefinitionSummary.Champion.Player.Id;
                viewModel.WinPercentage = gameDefinitionSummary.Champion.WinPercentage;
                viewModel.NumberOfGamesPlayed = gameDefinitionSummary.Champion.NumberOfGames;
                viewModel.NumberOfWins = gameDefinitionSummary.Champion.NumberOfWins;
            }
        }

        private void SetPlayedGames(GameDefinitionSummary gameDefinitionSummary, ApplicationUser currentUser,
            GameDefinitionDetailsViewModel viewModel)
        {
            if (gameDefinitionSummary.PlayedGames == null)
            {
                viewModel.PlayedGames = new List<PlayedGameDetailsViewModel>();
            }
            else
            {
                viewModel.PlayedGames = (from playedGame in gameDefinitionSummary.PlayedGames
                        select _playedGameDetailsViewModelBuilder.Build(playedGame, currentUser, true))
                    .ToList();
            }
        }
    }
}
