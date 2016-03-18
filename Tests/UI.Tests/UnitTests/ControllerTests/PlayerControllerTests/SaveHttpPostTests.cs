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
using System.Web.Routing;
using BusinessLogic.Exceptions;
using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;
using System.Web.Mvc;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;

namespace UI.Tests.UnitTests.ControllerTests.PlayerControllerTests
{
    [TestFixture]
    public class SaveHttpPostTests : PlayerControllerTestBase
    {
        [Test]
        public void ItSavesThePlayer()
        {
            var player = new CreatePlayerRequest
            {
                Name = "player name"
            };

            autoMocker.ClassUnderTest.Save(player, currentUser);

            autoMocker.Get<IPlayerSaver>().AssertWasCalled(mock => mock.CreatePlayer(
                Arg<CreatePlayerRequest>.Matches(
                    x => x.Name == player.Name 
                    && x.GamingGroupId == currentUser.CurrentGamingGroupId), 
                Arg<ApplicationUser>.Is.Equal(currentUser),
                Arg<bool>.Is.Equal(false)));
        }

        [Test]
        public void ItReturnsBadRequestWhenTheRequestIsNotAjax()
        {
            var context = MockRepository.GenerateMock<HttpContextBase>();
            context.Expect(x => x.Request)
                .Repeat.Any()
                .Return(autoMocker.Get<HttpRequestBase>());
            autoMocker.Get<HttpRequestBase>().Headers.Clear();

            autoMocker.ClassUnderTest.ControllerContext = new ControllerContext(context, new RouteData(), autoMocker.ClassUnderTest);

            var result = autoMocker.ClassUnderTest.Save(new CreatePlayerRequest(), currentUser) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public void ItReturnsANotModifiedStatusIfValidationFails()
        {
            var player = new CreatePlayerRequest();
            autoMocker.ClassUnderTest.ModelState.AddModelError("key", "message");

            var result = autoMocker.ClassUnderTest.Save(player, currentUser) as HttpStatusCodeResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.NotModified, result.StatusCode);
        }

        [Test]
        public void ItReturnsAConflictHttpStatusCodeWhenThePlayerExists()
        {
            var player = new CreatePlayerRequest
            {
                Name = "player name"
            };
            autoMocker.Get<IPlayerSaver>().Expect(x => x.CreatePlayer(player, currentUser))
                .Repeat.Once()
                .Throw(new PlayerAlreadyExistsException(player.Name, 1));

            var result = autoMocker.ClassUnderTest.Save(player, currentUser) as HttpStatusCodeResult;

            Assert.IsNotNull(result);
            Assert.AreEqual((int)HttpStatusCode.Conflict, result.StatusCode);

        }
    }
}
