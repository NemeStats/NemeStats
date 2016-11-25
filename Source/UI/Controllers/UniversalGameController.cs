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

using System.Linq;
using BusinessLogic.Models.User;
using System.Web.Mvc;
using BusinessLogic.Logic;
using BusinessLogic.Logic.BoardGameGeekGameDefinitions;
using UI.Attributes.Filters;
using UI.Controllers.Helpers;
using UI.Models.GameDefinitionModels;
using UI.Models.Players;
using UI.Models.UniversalGameModels;
using UI.Transformations;

namespace UI.Controllers
{
    public partial class UniversalGameController : BaseController
    {
        private readonly ITransformer _transformer;
        private readonly IUniversalGameRetriever _universalGameRetriever;
        private readonly IPlayedGameDetailsViewModelBuilder _playedGameDetailsViewModelBuilder;

        public UniversalGameController(ITransformer transformer, IUniversalGameRetriever universalGameRetriever, IPlayedGameDetailsViewModelBuilder playedGameDetailsViewModelBuilder, IUniversalTopChampionsRetreiver universalTopChampionsRetreiver)
        {
            _transformer = transformer;
            _universalGameRetriever = universalGameRetriever;
            _playedGameDetailsViewModelBuilder = playedGameDetailsViewModelBuilder;
        }

        [UserContext(RequiresGamingGroup = false)]
        public virtual ActionResult Details(int id, ApplicationUser currentUser)
        {
            var boardGameGeekGameSummary = _universalGameRetriever.GetBoardGameGeekGameSummary(id, currentUser);
            var viewModel = _transformer.Transform<UniversalGameDetailsViewModel>(boardGameGeekGameSummary);
            viewModel.BoardGameGeekInfo.HideLinkToGlobalStats = true;

            var gamingGroupGameDefinitionSummary = boardGameGeekGameSummary.GamingGroupGameDefinitionSummary;

            if (gamingGroupGameDefinitionSummary != null)
            {
                viewModel.GamingGroupGameDefinitionSummary = new GamingGroupGameDefinitionViewModel
                {
                    GamingGroupId = gamingGroupGameDefinitionSummary.GamingGroupId,
                    GamingGroupName = gamingGroupGameDefinitionSummary.GamingGroupName,
                    PlayedGamesPanelTitle = $"Last {gamingGroupGameDefinitionSummary.PlayedGames.Count} Played Games",
                    PlayedGames = gamingGroupGameDefinitionSummary.PlayedGames.Select(playedGame => _playedGameDetailsViewModelBuilder.Build(playedGame, currentUser)).ToList(),
                    GameDefinitionPlayerSummaries = gamingGroupGameDefinitionSummary.PlayerWinRecords
                    .Select(playerWinRecord => _transformer.Transform<GameDefinitionPlayerSummaryViewModel>(playerWinRecord)).ToList(),
                    GamingGroupGameDefinitionStats = new GamingGroupGameDefinitionStatsViewModel
                    {
                        AveragePlayersPerGame = $"{gamingGroupGameDefinitionSummary.AveragePlayersPerGame:0.#}",
                        TotalNumberOfGamesPlayed = gamingGroupGameDefinitionSummary.TotalNumberOfGamesPlayed
                    }
                };
            }

            return View(MVC.UniversalGame.Views.Details, viewModel);
        }
    }
}
