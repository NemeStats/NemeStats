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
            HttpStatusCodeResult result = playerController.Edit(nullPlayerId) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public void ItReturnsAnUnauthorizedAccessHttpStatusCodeIfTheUserDoesntHaveAccess()
        {
            int playerId = 1;
            playerRetrieverMock.Expect(mock => mock.GetPlayerDetails(playerId, 0))
                .Throw(new UnauthorizedAccessException());
            HttpStatusCodeResult result = playerController.Edit(playerId) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Test]
        public void ItReturnsANotFoundHttpStatusCodeIfThePlayerDoesntExist()
        {
            int playerId = -1;
            playerRetrieverMock.Expect(mock => mock.GetPlayerDetails(playerId, 0))
                .Throw(new KeyNotFoundException());
            HttpStatusCodeResult result = playerController.Edit(playerId) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.NotFound, result.StatusCode);
        }

        [Test]
        public void ItLoadsTheEditView()
        {
            int playerId = 123;
            playerRetrieverMock.Expect(mock => mock.GetPlayerDetails(playerId, 0))
                .Return(new PlayerDetails());

            PlayerDetails playerDetails = new PlayerDetails
            {
                Id = playerId
            };

            ViewResult result = playerController.Edit(playerDetails.Id) as ViewResult;

            Assert.AreEqual(MVC.Player.Views.Edit, result.ViewName);
        }

        [Test]
        public void ItPutsThePlayerEditViewModelOnTheView()
        {
            PlayerDetails playerDetails = new PlayerDetails();
            playerRetrieverMock.Expect(mock => mock.GetPlayerDetails(playerDetails.Id, 0))
                .Repeat.Once()
                .Return(playerDetails);

            ViewResult result = playerController.Edit(playerDetails.Id) as ViewResult;

            var actualViewModel = (PlayerEditViewModel)result.Model;

            Assert.That(playerDetails.Name, Is.EqualTo(actualViewModel.Name));
            Assert.That(playerDetails.Id, Is.EqualTo(actualViewModel.Id));
            Assert.That(playerDetails.GamingGroupId, Is.EqualTo(actualViewModel.GamingGroupId));
            Assert.That(playerDetails.Active, Is.EqualTo(actualViewModel.Active));

        }
    }
}
