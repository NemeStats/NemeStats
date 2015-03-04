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
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
    [TestFixture]
    public class EditHttpGetTests : GameDefinitionControllerTestBase
    {
        [Test]
        public void ItReturnsAnEditView()
        {
            int gameDefinitionId = 111;
            ViewResult viewResult = gameDefinitionControllerPartialMock.Edit(gameDefinitionId, currentUser) as ViewResult;

            Assert.AreEqual(MVC.GameDefinition.Views.Edit, viewResult.ViewName);
        }

        [Test]
        public void ItSetsTheGameDefinitionOnTheView()
        {
            GameDefinitionSummary gameDefinitionSummary = new GameDefinitionSummary()
            {
                Id = 135
            };
            gameDefinitionRetrieverMock.Expect(mock => mock.GetGameDefinitionDetails(gameDefinitionSummary.Id, 0))
                .Repeat.Once()
                .Return(gameDefinitionSummary);

            ViewResult viewResult = gameDefinitionControllerPartialMock.Edit(gameDefinitionSummary.Id, currentUser) as ViewResult;

            Assert.AreEqual(gameDefinitionSummary, viewResult.ViewData.Model);
        }

        [Test]
        public void ItReturnsABadRequestHttpStatusCodeIfNoIdIsPassed()
        {
            int? nullGameId = null;
            HttpStatusCodeResult statusCodeResult = gameDefinitionControllerPartialMock.Edit(nullGameId, currentUser) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, statusCodeResult.StatusCode);
        }

        [Test]
        public void ItReturnsANotFoundHttpStatusCodeIfNoGameDefinitionIsFound()
        {
            int gameDefinitionId = -1;
            gameDefinitionRetrieverMock.Expect(mock => mock.GetGameDefinitionDetails(gameDefinitionId, 0))
                .Repeat.Once()
                .Throw(new KeyNotFoundException());
            HttpStatusCodeResult statusCodeResult = gameDefinitionControllerPartialMock.Edit(gameDefinitionId, currentUser) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.NotFound, statusCodeResult.StatusCode);
        }
    }
}
