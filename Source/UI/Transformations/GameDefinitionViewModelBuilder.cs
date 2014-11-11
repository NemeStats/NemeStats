using System.Collections.Generic;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using System.Linq;
using UI.Models.GameDefinitionModels;
using UI.Models.PlayedGame;

namespace UI.Transformations
{
    public class GameDefinitionViewModelBuilder : IGameDefinitionViewModelBuilder
    {
        internal IPlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilder;

        public GameDefinitionViewModelBuilder(IPlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilder)
        {
            this.playedGameDetailsViewModelBuilder = playedGameDetailsViewModelBuilder;
        }

        public GameDefinitionViewModel Build(GameDefinitionSummary gameDefinitionSummary, ApplicationUser currentUser)
        {
            GameDefinitionViewModel viewModel = new GameDefinitionViewModel()
            {
                Id = gameDefinitionSummary.Id,
                Name = gameDefinitionSummary.Name,
                Description = gameDefinitionSummary.Description,
                TotalNumberOfGamesPlayed = gameDefinitionSummary.TotalNumberOfGamesPlayed,
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
