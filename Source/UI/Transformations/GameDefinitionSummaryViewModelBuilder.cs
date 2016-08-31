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

using BusinessLogic.Logic.Points;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using UI.Models.GameDefinitionModels;

namespace UI.Transformations
{
    public class GameDefinitionSummaryViewModelBuilder : IGameDefinitionSummaryViewModelBuilder
    {
        private readonly ITransformer _transformer;
        private readonly IWeightTierCalculator _weightTierCalculator;

        public GameDefinitionSummaryViewModelBuilder(ITransformer transformer, IWeightTierCalculator weightTierCalculator)
        {
            this._transformer = transformer;
            _weightTierCalculator = weightTierCalculator;
        }

        public GameDefinitionSummaryViewModel Build(GameDefinitionSummary gameDefinitionSummary, ApplicationUser currentUser)
        {
            var viewModel = new GameDefinitionDetailsViewModel
            {
                Id = gameDefinitionSummary.Id,
                Name = gameDefinitionSummary.Name,
                Description = gameDefinitionSummary.Description,
                TotalNumberOfGamesPlayed = gameDefinitionSummary.TotalNumberOfGamesPlayed,
                GamingGroupId = gameDefinitionSummary.GamingGroupId,
                GamingGroupName = gameDefinitionSummary.GamingGroupName,
                BoardGameGeekGameDefinition = _transformer.Transform<BoardGameGeekGameDefinitionViewModel>(gameDefinitionSummary.BoardGameGeekGameDefinition),
                UserCanEdit =
                    (currentUser != null && gameDefinitionSummary.GamingGroupId == currentUser.CurrentGamingGroupId)
            };

            if (viewModel.BoardGameGeekGameDefinition != null)
            {
                viewModel.BoardGameGeekGameDefinition.WeightDescription = _weightTierCalculator.GetWeightTier(viewModel.BoardGameGeekGameDefinition.AverageWeight).ToString();
            }

            if (!(gameDefinitionSummary.Champion is NullChampion))
            {
                viewModel.ChampionName = gameDefinitionSummary.Champion.Player.Name;
                viewModel.ChampionPlayerId = gameDefinitionSummary.Champion.Player.Id;
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