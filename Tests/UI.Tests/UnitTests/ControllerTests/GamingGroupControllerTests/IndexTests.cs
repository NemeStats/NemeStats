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
using UI.Controllers;
using UI.Models.GamingGroup;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class IndexTests : GamingGroupControllerTestBase
    {
        private GamingGroup gamingGroup;
        private GamingGroupViewModel gamingGroupViewModel;

        [Test]
        public override void SetUp()
        {
            base.SetUp();

            gamingGroup = new GamingGroup()
            {
                PlayedGames = new List<PlayedGame>()
            };
            gamingGroupViewModel = new GamingGroupViewModel();

            gamingGroupRetrieverMock.Expect(mock => mock.GetGamingGroupDetails(
                currentUser.CurrentGamingGroupId.Value,
                GamingGroupController.MAX_NUMBER_OF_RECENT_GAMES))
                .Repeat.Once()
                .Return(gamingGroup);

            gamingGroupViewModelBuilderMock.Expect(mock => mock.Build(gamingGroup, currentUser))
                .Return(gamingGroupViewModel);
        }

        [Test]
        public void ItReturnsTheIndexView()
        {
            ViewResult viewResult = gamingGroupController.Index(currentUser) as ViewResult;

            Assert.AreEqual(MVC.GamingGroup.Views.Index, viewResult.ViewName);
        }

        [Test]
        public void ItAddsAGamingGroupViewModelToTheView()
        {
            ViewResult viewResult = gamingGroupController.Index(currentUser) as ViewResult;

            Assert.AreSame(gamingGroupViewModel, viewResult.Model);
        }

        [Test]
        public void ItAddsTheRecentlyPlayedGamesMessageToTheViewBag()
        {
            string expectedMessage = "expected message";
            showingXResultsMessageBuilderMock.Expect(mock => mock.BuildMessage(
                 GamingGroupController.MAX_NUMBER_OF_RECENT_GAMES,
                 gamingGroup.PlayedGames.Count))
                     .Return(expectedMessage);

            ViewResult viewResult = gamingGroupController.Index(currentUser) as ViewResult;

            Assert.AreEqual(expectedMessage, gamingGroupController.ViewBag.RecentGamesMessage);
        }
    }
}
