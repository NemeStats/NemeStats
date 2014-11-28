using BusinessLogic.Models;
using BusinessLogic.Models.GamingGroups;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UI.Controllers;
using UI.Models.GamingGroup;

namespace UI.Tests.UnitTests.ControllerTests.GamingGroupControllerTests
{
    [TestFixture]
    public class IndexTests : GamingGroupControllerTestBase
    {
        private GamingGroupSummary gamingGroupSummary;
        private GamingGroupViewModel gamingGroupViewModel;

        [Test]
        public override void SetUp()
        {
            base.SetUp();

            gamingGroupSummary = new GamingGroupSummary()
            {
                PlayedGames = new List<PlayedGame>()
            };
            gamingGroupViewModel = new GamingGroupViewModel();

            gamingGroupRetrieverMock.Expect(mock => mock.GetGamingGroupDetails(
                currentUser.CurrentGamingGroupId.Value,
                GamingGroupController.MAX_NUMBER_OF_RECENT_GAMES))
                .Repeat.Once()
                .Return(gamingGroupSummary);

            gamingGroupViewModelBuilderMock.Expect(mock => mock.Build(gamingGroupSummary, currentUser))
                .Return(gamingGroupViewModel);
        }

        [Test]
        public void ItReturnsTheIndexView()
        {
            ViewResult viewResult = gamingGroupControllerPartialMock.Index(currentUser) as ViewResult;

            Assert.AreEqual(MVC.GamingGroup.Views.Index, viewResult.ViewName);
        }

        [Test]
        public void ItAddsAGamingGroupViewModelToTheView()
        {
            ViewResult viewResult = gamingGroupControllerPartialMock.Index(currentUser) as ViewResult;

            Assert.AreSame(gamingGroupViewModel, viewResult.Model);
        }

        [Test]
        public void ItAddsTheRecentlyPlayedGamesMessageToTheViewBag()
        {
            string expectedMessage = "expected message";
            showingXResultsMessageBuilderMock.Expect(mock => mock.BuildMessage(
                 GamingGroupController.MAX_NUMBER_OF_RECENT_GAMES,
                 gamingGroupSummary.PlayedGames.Count))
                     .Return(expectedMessage);

            ViewResult viewResult = gamingGroupControllerPartialMock.Index(currentUser) as ViewResult;

            Assert.AreEqual(expectedMessage, gamingGroupControllerPartialMock.ViewBag.RecentGamesMessage);
        }
    }
}
