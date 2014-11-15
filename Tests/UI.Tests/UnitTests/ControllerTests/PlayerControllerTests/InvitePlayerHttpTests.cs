using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BusinessLogic.Models.Players;
using NUnit.Framework;
using Rhino.Mocks;
using UI.Controllers;
using UI.Models.Players;

namespace UI.Tests.UnitTests.ControllerTests.PlayerControllerTests
{
    [TestFixture]
    public class InvitePlayerHttpTests : PlayerControllerTestBase
    {
        private PlayerDetails playerDetails;
        private int playerId = 915;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            playerDetails = new PlayerDetails
            {
                Name = "player name",
                Id = playerId
            };
            playerRetrieverMock.Expect(mock => mock.GetPlayerDetails(playerId, 0))
                              .Return(playerDetails);
        }

        [Test]
        public void ItReturnsNotFoundHttpStatusWhenInvalidPlayerIdGiven()
        {
            int invalidPlayerId = -1;
            playerRetrieverMock.Expect(mock => mock.GetPlayerDetails(invalidPlayerId, 0))
                               .Throw(new KeyNotFoundException());

            HttpStatusCodeResult actualResult = playerController.InvitePlayer(-1, currentUser) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.NotFound, actualResult.StatusCode);
        }

        [Test]
        public void ItSetsThePlayerNameAndIdOnTheViewModel()
        {
            ViewResult viewResult = playerController.InvitePlayer(playerId, currentUser) as ViewResult;

            Assert.AreEqual(playerDetails.Name, ((PlayerInvitationViewModel)viewResult.Model).PlayerName);
            Assert.AreEqual(playerDetails.Id, ((PlayerInvitationViewModel)viewResult.Model).PlayerId);
        }

        [Test]
        public void ItDefaultsTheEmailSubjectToSayThatSomeoneHasInvitedThem()
        {
            string expectedEmailSubject = string.Format(PlayerController.EMAIL_SUBJECT_PLAYER_INVITATION, currentUser.UserName);

            ViewResult viewResult = playerController.InvitePlayer(playerId, currentUser) as ViewResult;

            Assert.AreEqual(expectedEmailSubject, ((PlayerInvitationViewModel)viewResult.Model).EmailSubject);
        }

        [Test]
        public void ItDefaultsTheEmailBodyToSayThatSomeoneHasInvitedThemToNemeStats()
        {
            string expectedEmailBody = string.Format(PlayerController.EMAIL_BODY_PLAYER_INVITATION, playerDetails.Name);

            ViewResult viewResult = playerController.InvitePlayer(playerId, currentUser) as ViewResult;

            Assert.AreEqual(expectedEmailBody, ((PlayerInvitationViewModel)viewResult.Model).EmailBody);
        }
    }
}
