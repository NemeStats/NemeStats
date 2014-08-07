using BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UI.Models.GameDefinitionModels;

namespace UI.Transformations
{
    public class GameDefinitionToGameDefinitionViewModelTransformationImpl : GameDefinitionToGameDefinitionViewModelTransformation
    {
        internal PlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilder;

        public GameDefinitionToGameDefinitionViewModelTransformationImpl(PlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilder)
        {
            this.playedGameDetailsViewModelBuilder = playedGameDetailsViewModelBuilder;
        }

        public GameDefinitionViewModel Build(GameDefinition gameDefinition)
        {
            GameDefinitionViewModel viewModel = new GameDefinitionViewModel()
            {
                Id = gameDefinition.Id,
                Name = gameDefinition.Name,
                Description = gameDefinition.Description
            };

            viewModel.PlayedGames = (from playedGame in gameDefinition.PlayedGames
                                     select playedGameDetailsViewModelBuilder.Build(playedGame))
                                    .ToList();
                                    
            return viewModel;
        }
    }
}
