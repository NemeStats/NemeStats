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
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using System.Linq;
using UI.Models.GameDefinitionModels;
using UI.Models.PlayedGame;
using UI.Models.Players;

namespace UI.Transformations
{
    public class GameDefinitionDetailsViewModelBuilder : IGameDefinitionDetailsViewModelBuilder
    {
        private readonly IPlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilder;
        private readonly ITransformer transformer;

        public GameDefinitionDetailsViewModelBuilder(IPlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilder, ITransformer transformer)
        {
            this.playedGameDetailsViewModelBuilder = playedGameDetailsViewModelBuilder;
            this.transformer = transformer;
        }

        public GameDefinitionDetailsViewModel Build(GameDefinitionSummary gameDefinitionSummary, ApplicationUser currentUser)
        {
            var viewModel = new GameDefinitionDetailsViewModel()
            {
                Id = gameDefinitionSummary.Id,
                Name = gameDefinitionSummary.Name,
                Description = gameDefinitionSummary.Description,
                TotalNumberOfGamesPlayed = gameDefinitionSummary.TotalNumberOfGamesPlayed,
                AveragePlayersPerGame = string.Format("{0:0.#}", gameDefinitionSummary.AveragePlayersPerGame),
                GamingGroupId = gameDefinitionSummary.GamingGroupId,
                GamingGroupName = gameDefinitionSummary.GamingGroupName,
                BoardGameGeekGameDefinitionId = gameDefinitionSummary.BoardGameGeekGameDefinitionId,
                BoardGameGeekUri = gameDefinitionSummary.BoardGameGeekUri,
                ThumbnailImageUrl = gameDefinitionSummary.ThumbnailImageUrl,
                UserCanEdit = (currentUser != null && gameDefinitionSummary.GamingGroupId == currentUser.CurrentGamingGroupId)
            };

            if (gameDefinitionSummary.PlayedGames == null)
            {
                viewModel.PlayedGames = new List<PlayedGameDetailsViewModel>();
            }
            else
            {
                viewModel.PlayedGames = (from playedGame in gameDefinitionSummary.PlayedGames
                                         select playedGameDetailsViewModelBuilder.Build(playedGame, currentUser))
                                   .ToList();
            }

            if (!(gameDefinitionSummary.Champion is NullChampion))
            {
                viewModel.ChampionName = gameDefinitionSummary.Champion.Player.Name;
                viewModel.ChampionPlayerId = gameDefinitionSummary.Champion.Player.Id;
                viewModel.WinPercentage = gameDefinitionSummary.Champion.WinPercentage;
                viewModel.NumberOfGamesPlayed = gameDefinitionSummary.Champion.NumberOfGames;
                viewModel.NumberOfWins = gameDefinitionSummary.Champion.NumberOfWins;
            }

            if (!(gameDefinitionSummary.PreviousChampion is NullChampion))
            {
                viewModel.PreviousChampionName = gameDefinitionSummary.PreviousChampion.Player.Name;
                viewModel.PreviousChampionPlayerId = gameDefinitionSummary.PreviousChampion.Player.Id;
            }

            var playersSummaryViewModel = new GameDefinitionPlayersSummaryViewModel
            {
                GameDefinitionPlayerSummaries = gameDefinitionSummary.PlayerWinRecords.Select(transformer.Transform<PlayerWinRecord, GameDefinitionPlayerSummaryViewModel>).ToList()
            };
            viewModel.GameDefinitionPlayersSummary = playersSummaryViewModel;

            return viewModel;
        }
    }
}
