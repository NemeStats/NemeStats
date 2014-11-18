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
        public const string BOARD_GAME_GEEK_BOARD_GAME_BASE_URI = "http://boardgamegeek.com/boardgame/{0}";

        //TODO Discuss with Tosho. How do I avoid duplicating this logic? If I call existing GameDefinitionSummaryViewModelBuilder
        //then I get back a GameDefinitionSummaryViewModel, not a GameDefinitionDetailsViewModel
        internal IPlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilder;

        public GameDefinitionDetailsViewModelBuilder(IPlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilder)
        {
            this.playedGameDetailsViewModelBuilder = playedGameDetailsViewModelBuilder;
        }

        public GameDefinitionDetailsViewModel Build(GameDefinitionSummary gameDefinitionSummary, ApplicationUser currentUser)
        {
            Uri boardGameGeekUri = null;

            if (gameDefinitionSummary.BoardGameGeekObjectId.HasValue)
            {
                boardGameGeekUri = new Uri(string.Format(BOARD_GAME_GEEK_BOARD_GAME_BASE_URI, gameDefinitionSummary.BoardGameGeekObjectId.Value));
            }
            GameDefinitionDetailsViewModel viewModel = new GameDefinitionDetailsViewModel()
            {
                Id = gameDefinitionSummary.Id,
                Name = gameDefinitionSummary.Name,
                Description = gameDefinitionSummary.Description,
                TotalNumberOfGamesPlayed = gameDefinitionSummary.TotalNumberOfGamesPlayed,
                GamingGroupId = gameDefinitionSummary.GamingGroupId,
                GamingGroupName = gameDefinitionSummary.GamingGroupName,
                BoardGameGeekUri = boardGameGeekUri,
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
                                  
            return viewModel;
        }
    }
}
