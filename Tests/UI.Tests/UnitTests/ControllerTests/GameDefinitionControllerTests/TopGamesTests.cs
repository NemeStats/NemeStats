using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Web.Mvc;
using Shouldly;
using UI.Controllers;
using UI.Models.GameDefinitionModels;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
    [TestFixture]
    public class TopGamesTests : GameDefinitionControllerTestBase
    {
        [Test]
        public void It_Returns_A_ViewResult_With_The_Specified_Number_Of_Trending_Games_Over_The_Specified_Number_Of_Days()
        {
            //--assert
            var expectedViewModels = new List<TrendingGameViewModel>();
            autoMocker.PartialMockTheClassUnderTest();
            autoMocker.ClassUnderTest.Expect(mock =>
                    mock.GetTrendingGamesViewModels(Arg<int>.Is.Anything, Arg<int>.Is.Anything))
                .Return(expectedViewModels);

            //--act
            var results = autoMocker.ClassUnderTest.TopGames();

            //--assert
            var viewResult = results as ViewResult;
            viewResult.ShouldNotBeNull();
            viewResult.ViewName.ShouldBe(MVC.GameDefinition.Views.TopGamesEver);
            autoMocker.ClassUnderTest.AssertWasCalled(mock =>
                mock.GetTrendingGamesViewModels(Arg<int>.Is.Equal(GameDefinitionController.NUMBER_OF_TOP_GAMES_EVER_TO_SHOW), Arg<int>.Is.Equal(GameDefinitionController.A_LOT_OF_DAYS)));

            viewResult.Model.ShouldBeSameAs(expectedViewModels);
        }
    }
}