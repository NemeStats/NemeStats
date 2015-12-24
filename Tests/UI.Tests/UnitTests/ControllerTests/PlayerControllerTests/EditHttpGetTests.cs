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
using BusinessLogic.Logic.Players;
using BusinessLogic.Models.Players;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using UI.Models.Players;

namespace UI.Tests.UnitTests.ControllerTests.PlayerControllerTests
{
    [TestFixture]
    public class EditHttpGetTests : PlayerControllerTestBase
    {
        [Test]
        public void ItReturnsABadRequestHttpStatusCodeIfThereIsNoPlayerId()
        {
            int? nullPlayerId = null;
            HttpStatusCodeResult result = autoMocker.ClassUnderTest.Edit(nullPlayerId) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public void ItReturnsAnUnauthorizedAccessHttpStatusCodeIfTheUserDoesntHaveAccess()
        {
            int playerId = 1;
            autoMocker.Get<IPlayerRetriever>().Expect(mock => mock.GetPlayerDetails(playerId, 0))
                .Throw(new UnauthorizedAccessException());
            HttpStatusCodeResult result = autoMocker.ClassUnderTest.Edit(playerId) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Test]
        public void ItReturnsANotFoundHttpStatusCodeIfThePlayerDoesntExist()
        {
            int playerId = -1;
            autoMocker.Get<IPlayerRetriever>().Expect(mock => mock.GetPlayerDetails(playerId, 0))
                .Throw(new KeyNotFoundException());
            HttpStatusCodeResult result = autoMocker.ClassUnderTest.Edit(playerId) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Test]
        public void ItLoadsTheEditView()
        {
            int playerId = 123;
            autoMocker.Get<IPlayerRetriever>().Expect(mock => mock.GetPlayerDetails(playerId, 0))
                .Return(new PlayerDetails());

            PlayerDetails playerDetails = new PlayerDetails
            {
                Id = playerId
            };

            ViewResult result = autoMocker.ClassUnderTest.Edit(playerDetails.Id) as ViewResult;

            Assert.AreEqual(MVC.Player.Views.Edit, result.ViewName);
        }

        [Test]
        public void ItPutsThePlayerEditViewModelOnTheView()
        {
            PlayerDetails playerDetails = new PlayerDetails();
            autoMocker.Get<IPlayerRetriever>().Expect(mock => mock.GetPlayerDetails(playerDetails.Id, 0))
                .Repeat.Once()
                .Return(playerDetails);

            ViewResult result = autoMocker.ClassUnderTest.Edit(playerDetails.Id) as ViewResult;

            var actualViewModel = (PlayerEditViewModel)result.Model;

            Assert.That(playerDetails.Name, Is.EqualTo(actualViewModel.Name));
            Assert.That(playerDetails.Id, Is.EqualTo(actualViewModel.Id));
            Assert.That(playerDetails.GamingGroupId, Is.EqualTo(actualViewModel.GamingGroupId));
            Assert.That(playerDetails.Active, Is.EqualTo(actualViewModel.Active));

        }
    }
}
