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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
    [TestFixture]
    public class SaveGameDefinitionPostTests : GameDefinitionControllerTestBase
    {
        [Test]
        public void ItSavesTheGame()
        {
            var game = new GameDefinition()
            {
                Name = "New Game"
            };

            gameDefinitionControllerPartialMock.Save(game, currentUser);

            gameDefinitionCreatorMock.AssertWasCalled(mock => mock.Save(game, currentUser));
        }

        [Test]
        public void ItReturnsBadRequestWhenTheRequestIsNotAjax()
        {
            var context = MockRepository.GenerateMock<HttpContextBase>();
            context.Expect(x => x.Request)
                .Repeat.Any()
                .Return(asyncRequestMock);
            asyncRequestMock.Headers.Clear();

            gameDefinitionControllerPartialMock.ControllerContext = new ControllerContext(context, new RouteData(), gameDefinitionControllerPartialMock);

            var result = gameDefinitionControllerPartialMock.Save(new GameDefinition(), currentUser) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public void ItReturnsANotModifiedStatusIfValidationFails()
        {
            var game = new GameDefinition();
            gameDefinitionControllerPartialMock.ModelState.AddModelError("key", "message");

            var result = gameDefinitionControllerPartialMock.Save(game, currentUser) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.NotModified, result.StatusCode);
        }
    }
}
