using System;
using System.Collections.Generic;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using System.Linq;
using UI.Models.GameDefinitionModels;
using UI.Models.PlayedGame;

namespace UI.Transformations
{
    public class GameDefinitionDetailsViewModelBuilder : IGameDefinitionDetailsViewModelBuilder
    {
        //TODO Discuss with Tosho. How do I avoid duplicating this logic? If I call existing GameDefinitionSummaryViewModelBuilder
        //then I get back a GameDefinitionSummaryViewModel, not a GameDefinitionDetailsViewModel
        internal IPlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilder;

        public GameDefinitionDetailsViewModelBuilder(IPlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilder)
        {
            this.playedGameDetailsViewModelBuilder = playedGameDetailsViewModelBuilder;
        }

        public GameDefinitionDetailsViewModel Build(GameDefinitionSummary gameDefinitionSummary, ApplicationUser currentUser)
        {
            GameDefinitionDetailsViewModel viewModel = new GameDefinitionDetailsViewModel()
            {
                Id = gameDefinitionSummary.Id,
                Name = gameDefinitionSummary.Name,
                Description = gameDefinitionSummary.Description,
                TotalNumberOfGamesPlayed = gameDefinitionSummary.TotalNumberOfGamesPlayed,
                GamingGroupId = gameDefinitionSummary.GamingGroupId,
                GamingGroupName = gameDefinitionSummary.GamingGroupName,
                BoardGameGeekObjectId = gameDefinitionSummary.BoardGameGeekObjectId,
                BoardGameGeekUri = gameDefinitionSummary.BoardGameGeekUri,
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

            return viewModel;
        }
    }
}
