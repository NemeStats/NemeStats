using System.Web.Mvc;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using UI.Controllers;

namespace UI.Tests.UnitTests.ControllerTests.HomeControllerTests
{
    public class TopGamesEverTests : HomeControllerTestBase
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
            var results = _autoMocker.ClassUnderTest.TopGamesEver();

            //--assert
            _autoMocker.ClassUnderTest.AssertWasCalled(mock => mock.GetTopGamesPartialView(
                Arg<int>.Is.Equal(HomeController.NUMBER_OF_TOP_GAMES_TO_SHOW), 
                Arg<int>.Is.Equal(HomeController.A_LOT_OF_DAYS)));
            results.ShouldBeSameAs(expectedResult);
        }
    }
}
