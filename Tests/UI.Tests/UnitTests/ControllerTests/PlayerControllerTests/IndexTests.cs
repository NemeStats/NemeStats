using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.PlayerControllerTests
{
    [TestFixture]
    public class IndexTests : PlayerControllerTestBase
    {
        [Test]
        public void ItReturnsTheIndexView()
        {
            ViewResult viewResult = playerController.Index(userContext) as ViewResult;

            Assert.AreEqual(MVC.Player.Views.Index, viewResult.ViewName);
        }

        [Test]
        public void ItAddsAllActivePlayersToTheView()
        {
            List<Player> players = new List<Player>();
            playerRepositoryMock.Expect(mock => mock.GetAllPlayers(true, userContext))
                .Repeat.Once()
                .Return(players);

            ViewResult viewResult = playerController.Index(userContext) as ViewResult;

            Assert.AreEqual(players, viewResult.Model);
        }
    }
}
