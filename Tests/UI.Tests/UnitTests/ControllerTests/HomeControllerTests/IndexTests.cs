using BusinessLogic.Models.Players;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using UI.Models.Home;
using UI.Models.Players;

namespace UI.Tests.UnitTests.ControllerTests.HomeControllerTests
{
    [TestFixture]
    public class IndexTests : HomeControllerTestBase
    {
        [Test]
        public void ItReturnsAnIndexView()
        {
            HomeIndexViewModel indexViewModel = new HomeIndexViewModel();
            List<TopPlayer> topPlayers = new List<TopPlayer>();
            playerSummaryBuilderMock.Expect(mock => mock.GetTopPlayers(Arg<int>.Is.Anything))
                .Return(topPlayers);
            TopPlayerViewModel expectedPlayer = new TopPlayerViewModel();
            viewModelBuilderMock.Expect(mock => mock.Build(Arg<TopPlayer>.Is.Anything))
                .Return(expectedPlayer);

            ViewResult viewResult = homeControllerPartialMock.Index() as ViewResult;

            Assert.AreEqual(MVC.Home.Views.Index, viewResult.ViewName);
        }

        [Test]
        public void TheIndexHasTheRecentPlayerSummaries()
        {
            HomeIndexViewModel indexViewModel = new HomeIndexViewModel();
            List<TopPlayer> topPlayers = new List<TopPlayer>()
            {
                new TopPlayer()
            };

            playerSummaryBuilderMock.Expect(mock => mock.GetTopPlayers(Arg<int>.Is.Anything))
                .Return(topPlayers);
            TopPlayerViewModel expectedPlayer = new TopPlayerViewModel();
            viewModelBuilderMock.Expect(mock => mock.Build(Arg<TopPlayer>.Is.Anything))
                .Return(expectedPlayer);

            ViewResult viewResult = homeControllerPartialMock.Index() as ViewResult;

            HomeIndexViewModel actualViewModel = (HomeIndexViewModel)viewResult.ViewData.Model;
            Assert.AreSame(expectedPlayer, actualViewModel.TopPlayers[0]);
        }
    }
}
