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
        public void It_Returns_The_Trending_Games_Partial_View_With_View_Models_For_Specific_Number_Of_Trending_Games()
        {
            //--arrange
            var expectedTrendingGames = new List<TrendingGame>
            {
                new TrendingGame(),
                new TrendingGame()
            };
            _autoMocker.Get<ITrendingGamesRetriever>()
                .Expect(mock => mock.GetResults(Arg<TrendingGamesRequest>.Is.Anything))
                .Return(expectedTrendingGames);
            var expectedViewModel1 = new TrendingGameViewModel();
            var expectedViewModel2 = new TrendingGameViewModel();
            _autoMocker.Get<ITransformer>().Expect(mock => mock.Transform<TrendingGameViewModel>(expectedTrendingGames[0])).Return(expectedViewModel1);
            _autoMocker.Get<ITransformer>().Expect(mock => mock.Transform<TrendingGameViewModel>(expectedTrendingGames[1])).Return(expectedViewModel2);

            //--act
            var results = _autoMocker.ClassUnderTest.TrendingGames();

            //--assert
            _autoMocker.Get<ITrendingGamesRetriever>().AssertWasCalled(
                mock => mock.GetResults(Arg<TrendingGamesRequest>.Matches(
                    x => x.NumberOfTrendingGamesToShow == HomeController.NUMBER_OF_TRENDING_GAMES_TO_SHOW 
                && x.NumberOfDaysOfTrendingGames == HomeController.NUMBER_OF_DAYS_OF_TRENDING_GAMES)));
            var viewResult = results as PartialViewResult;
            viewResult.ShouldNotBeNull();
            var viewModel = viewResult.Model as List<TrendingGameViewModel>;
            viewModel.ShouldNotBeNull();
            viewResult.ViewName.ShouldBe(MVC.GameDefinition.Views._TrendingGamesPartial);
            viewModel[0].ShouldBeSameAs(expectedViewModel1);
            viewModel[1].ShouldBeSameAs(expectedViewModel2);
        }

    }
}
