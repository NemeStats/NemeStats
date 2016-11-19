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

using BusinessLogic.Models.User;
using System.Web.Mvc;
using BusinessLogic.Logic;
using BusinessLogic.Logic.BoardGameGeekGameDefinitions;
using UI.Attributes.Filters;
using UI.Controllers.Helpers;
using UI.Models.UniversalGameModels;

namespace UI.Controllers
{
    public partial class UniversalGameController : BaseController
    {
        private readonly ITransformer _transformer;
        private readonly IUniversalGameRetriever _universalGameRetriever;

        public UniversalGameController(ITransformer transformer, IUniversalGameRetriever universalGameRetriever)
        {
            _transformer = transformer;
            _universalGameRetriever = universalGameRetriever;
        }

        [UserContext(RequiresGamingGroup = false)]
        public virtual ActionResult Details(int id, ApplicationUser currentUser)
        {
            var boardGameGeekGameSummary = _universalGameRetriever.GetBoardGameGeekGameSummary(id, currentUser);
            var viewModel = _transformer.Transform<UniversalGameDetailsViewModel>(boardGameGeekGameSummary);

            return View(MVC.UniversalGame.Views.Details, viewModel);
        }
    }
}
