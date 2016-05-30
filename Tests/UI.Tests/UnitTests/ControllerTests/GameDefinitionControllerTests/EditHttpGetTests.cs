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

using BusinessLogic.Models.Games;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using BusinessLogic.Logic.GameDefinitions;
using UI.Models.GameDefinitionModels;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
    [TestFixture]
    public class EditHttpGetTests : GameDefinitionControllerTestBase
    {
        [Test]
        public void ItReturnsAnEditView()
        {
            var gameDefinitionId = 111;
            var viewResult = autoMocker.ClassUnderTest.Edit(gameDefinitionId, currentUser) as ViewResult;

            Assert.AreEqual(MVC.GameDefinition.Views.Edit, viewResult.ViewName);
        }

        [Test]
        public void ItSetsTheGameDefinitionEditViewModelOnTheView()
        {
            var gameDefinitionSummary = new GameDefinitionSummary()
            {
                Id = 1,
                Name = "some name",
                Active = false,
                BoardGameGeekGameDefinitionId = 2,
                //TODO add thumbnail image url to the edit page
                //ThumbnailImageUrl = "some url",
                Description = "some description"
            };
            autoMocker.Get<IGameDefinitionRetriever>().BackToRecord(BackToRecordOptions.All);
            autoMocker.Get<IGameDefinitionRetriever>().Replay();
            autoMocker.Get<IGameDefinitionRetriever>().Expect(mock => mock.GetGameDefinitionDetails(gameDefinitionSummary.Id, 0))
                .Repeat.Once()
                .Return(gameDefinitionSummary);

            var viewResult = autoMocker.ClassUnderTest.Edit(gameDefinitionSummary.Id, currentUser) as ViewResult;
            var gameDefinitionEditViewModel = viewResult.ViewData.Model as GameDefinitionEditViewModel;
            Assert.That(gameDefinitionEditViewModel, Is.Not.Null);
            Assert.That(gameDefinitionEditViewModel.GameDefinitionId, Is.EqualTo(gameDefinitionSummary.Id));
            Assert.That(gameDefinitionEditViewModel.Name, Is.EqualTo(gameDefinitionSummary.Name));
            Assert.That(gameDefinitionEditViewModel.Active, Is.EqualTo(gameDefinitionSummary.Active));
            Assert.That(gameDefinitionEditViewModel.BoardGameGeekGameDefinitionId, Is.EqualTo(gameDefinitionSummary.BoardGameGeekGameDefinitionId));
        }

        [Test]
        public void ItReturnsABadRequestHttpStatusCodeIfNoIdIsPassed()
        {
            int? nullGameId = null;
            var statusCodeResult = autoMocker.ClassUnderTest.Edit(nullGameId, currentUser) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, statusCodeResult.StatusCode);
        }

        [Test]
        public void ItReturnsANotFoundHttpStatusCodeIfNoGameDefinitionIsFound()
        {
            var gameDefinitionId = -1;
            autoMocker.Get<IGameDefinitionRetriever>().BackToRecord(BackToRecordOptions.All);
            autoMocker.Get<IGameDefinitionRetriever>().Replay();
            autoMocker.Get<IGameDefinitionRetriever>().Expect(mock => mock.GetGameDefinitionDetails(gameDefinitionId, 0))
                .Repeat.Once()
                .Throw(new KeyNotFoundException());
            var statusCodeResult = autoMocker.ClassUnderTest.Edit(gameDefinitionId, currentUser) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.NotFound, statusCodeResult.StatusCode);
        }
    }
}
