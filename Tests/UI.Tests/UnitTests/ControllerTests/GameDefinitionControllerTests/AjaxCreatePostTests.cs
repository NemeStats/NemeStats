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

using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Models.GameDefinitionModels;
using BusinessLogic.Models.User;
using BusinessLogic.Models.Games;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.GameDefinitions;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
    [TestFixture]
    public class AjaxCreatePostTests : GameDefinitionControllerTestBase
    {
        [Test]
        public void ItCreatesTheNewGame()
        {
            var createGameDefinitionViewModel = new CreateGameDefinitionViewModel()
            {
                Active = false,
                BoardGameGeekGameDefinitionId = 1,
                Description = "some description",
                Name = "New Game"
            };

            autoMocker.ClassUnderTest.AjaxCreate(createGameDefinitionViewModel, currentUser);

            var arguments = autoMocker.Get<IGameDefinitionSaver>().GetArgumentsForCallsMadeOn(mock => mock.CreateGameDefinition(
                Arg<CreateGameDefinitionRequest>.Is.Anything, 
                Arg<ApplicationUser>.Is.Anything));
            var createGameDefinitionRequest = arguments[0][0] as CreateGameDefinitionRequest;
            Assert.That(createGameDefinitionRequest, Is.Not.Null);
            Assert.That(createGameDefinitionRequest.Active, Is.EqualTo(createGameDefinitionViewModel.Active));
            Assert.That(createGameDefinitionRequest.BoardGameGeekGameDefinitionId, Is.EqualTo(createGameDefinitionViewModel.BoardGameGeekGameDefinitionId));
            Assert.That(createGameDefinitionRequest.Description, Is.EqualTo(createGameDefinitionViewModel.Description));
            Assert.That(createGameDefinitionRequest.Name, Is.EqualTo(createGameDefinitionViewModel.Name));
        }

        [Test]
        public void ItReturnsBadRequestWhenTheRequestIsNotAjax()
        {
            var context = MockRepository.GenerateMock<HttpContextBase>();
            context.Expect(x => x.Request)
                .Repeat.Any()
                .Return(asyncRequestMock);
            asyncRequestMock.Headers.Clear();

            autoMocker.ClassUnderTest.ControllerContext = new ControllerContext(context, new RouteData(), autoMocker.ClassUnderTest);

            var result = autoMocker.ClassUnderTest.AjaxCreate(new CreateGameDefinitionViewModel(), currentUser) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public void ItReturnsABadRequestWithTheTheFirstModelStateErrorValidationMessageIfValidationFails()
        {
            var model = new CreateGameDefinitionViewModel();
            const string MESSAGE = "some message";
            autoMocker.ClassUnderTest.ModelState.AddModelError("key", MESSAGE);

            var result = autoMocker.ClassUnderTest.AjaxCreate(model, currentUser) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.AreEqual(result.StatusDescription, MESSAGE);
        }

        [Test]
        public void ItReturnsAFriendlyMessageIfThereIsADuplicateKeyException()
        {
            var model = new CreateGameDefinitionViewModel
            {
                Name = "some name"
            };
            autoMocker.Get<IGameDefinitionSaver>().Expect(mock => mock.CreateGameDefinition(null, null))
                .IgnoreArguments()
                .Throw(new DuplicateKeyException("some duplicate key"));

            var result = autoMocker.ClassUnderTest.AjaxCreate(model, currentUser) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
            Assert.AreEqual("This Game Definition is already active within your Gaming Group.", result.StatusDescription);
        }
    }
}
