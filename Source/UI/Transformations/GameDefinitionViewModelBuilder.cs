using BusinessLogic.Models;
using BusinessLogic.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UI.Models.GameDefinitionModels;

namespace UI.Transformations
{
    public class GameDefinitionViewModelBuilder : IGameDefinitionViewModelBuilder
    {
        internal IPlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilder;

        public GameDefinitionViewModelBuilder(IPlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilder)
        {
            this.playedGameDetailsViewModelBuilder = playedGameDetailsViewModelBuilder;
        }

        public GameDefinitionViewModel Build(GameDefinition gameDefinition, ApplicationUser currentUser)
        {
            GameDefinitionViewModel viewModel = new GameDefinitionViewModel()
            {
                Id = gameDefinition.Id,
                Name = gameDefinition.Name,
                Description = gameDefinition.Description,
                UserCanEdit = (gameDefinition.GamingGroupId == currentUser.CurrentGamingGroupId)
            };

            viewModel.PlayedGames = (from playedGame in gameDefinition.PlayedGames
                                     select playedGameDetailsViewModelBuilder.Build(playedGame, currentUser))
                                    .ToList();
                                    
            return viewModel;
        }
    }
}
