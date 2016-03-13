using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Web.Mvc;
using UI.Models.GameDefinitionModels;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
    [TestFixture]
    public class ShowTrendingGamesTests : GameDefinitionControllerTestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            trendingGamesGameViewModels = new List<TrendingGameViewModel>();

            autoMocker.PartialMockTheClassUnderTest();
            autoMocker.ClassUnderTest.Expect(mock => mock.ShowTrendingGames()).Return(new ViewResult { ViewName = MVC.GameDefinition.Views.TrendingGames, ViewData = new ViewDataDictionary(trendingGamesGameViewModels) });
        }

        [Test]
        public void ItReturnsTrendingGamesView()
        {
            var viewResult = autoMocker.ClassUnderTest.ShowTrendingGames() as ViewResult;

            Assert.AreEqual(MVC.GameDefinition.Views.TrendingGames, viewResult.ViewName);
        }

        [Test]
        public void ItReturnsSpecifiedTrendingGamesViewModelToView()
        {
            var viewResult = autoMocker.ClassUnderTest.ShowTrendingGames() as ViewResult;

            var actualModel = viewResult.ViewData.Model;

            Assert.AreEqual(trendingGamesGameViewModels, actualModel);
        }
    }
}