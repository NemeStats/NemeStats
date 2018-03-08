using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic.Facades;
using BusinessLogic.Logic;
using BusinessLogic.Models.Games;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using UI.Controllers;
using UI.Models.GameDefinitionModels;

namespace UI.Tests.UnitTests.ControllerTests.HomeControllerTests
{
    public class TrendingGamesTests : HomeControllerTestBase
    {
        [Test]
        public void It_Returns_A_Partial_View_For_The_Specified_Number_Of_Days()
        {
            //--arrange
            _autoMocker.PartialMockTheClassUnderTest();

            var expectedResult = new PartialViewResult();
            _autoMocker.ClassUnderTest.Expect(mock => mock.GetTopGamesPartialView(Arg<int>.Is.Anything, Arg<int>.Is.Anything))
                .Return(expectedResult);

            //--act
            var results = _autoMocker.ClassUnderTest.TrendingGames();

            //--assert
            _autoMocker.ClassUnderTest.AssertWasCalled(mock => mock.GetTopGamesPartialView(
                Arg<int>.Is.Equal(HomeController.NUMBER_OF_TRENDING_GAMES_TO_SHOW), 
                Arg<int>.Is.Equal(HomeController.NUMBER_OF_DAYS_OF_TRENDING_GAMES)));
            results.ShouldBeSameAs(expectedResult);
        }
    }
}
