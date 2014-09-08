using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using UI.Controllers;

namespace UI.Tests.UnitTests.ControllerTests.PlayerControllerTests
{
    [TestFixture]
    public class DeleteConfirmedTests : PlayerControllerTestBase
    {
        [Test]
        public void ItDeletesThePlayer()
        {
            int playerId = 1;
            playerController.DeleteConfirmed(playerId, currentUser);

            dataContextMock.AssertWasCalled(mock => mock.DeleteById<Player>(playerId, currentUser));
        }

        [Test]
        public void ItReturnsAnUnauthorizedAccessHttpStatusIfTheUserDoesntHaveAccess()
        {
            int playerId = 1351;
            dataContextMock.Expect(mock => mock.DeleteById<Player>(playerId, currentUser))
                .Throw(new UnauthorizedAccessException());
            HttpStatusCodeResult result = playerController.DeleteConfirmed(playerId, currentUser) as HttpStatusCodeResult;

            Assert.AreEqual((int)HttpStatusCode.Unauthorized, result.StatusCode);
        }

        [Test]
        public void ItRedirectsToTheGamingGroupIndexAndPlayersSectionAfterSaving()
        {
            int playerId = 1;
            dataContextMock.Expect(mock => mock.DeleteById<Player>(playerId, currentUser));
            string baseUrl = "base url";
            string expectedUrl = baseUrl + "#" + GamingGroupController.SECTION_ANCHOR_PLAYERS;
            urlHelperMock.Expect(mock => mock.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name))
                    .Return(baseUrl);

            RedirectResult redirectResult = playerController.DeleteConfirmed(playerId, currentUser) as RedirectResult;

            Assert.AreEqual(expectedUrl, redirectResult.Url);
        }
    }
}
