using System.Collections.Generic;
using BusinessLogic.Facades;
using BusinessLogic.Logic;
using BusinessLogic.Models.Games;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using UI.Models.GameDefinitionModels;

namespace UI.Tests.UnitTests.ControllerTests.HomeControllerTests
{
    public class GetTopGamesPartialViewTests : HomeControllerTestBase
    {
        [Test]
        public void It_Returns_The_Trending_Games_Partial_View_With_View_Models_For_Specific_Number_Of_Trending_Games()
        {
            //--arrange
            var numberOfGamesToShow = 1;
            var numberOfDaysToConsider = 2;
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
            var results = _autoMocker.ClassUnderTest.GetTopGamesPartialView(numberOfGamesToShow, numberOfDaysToConsider);

            //--assert
            _autoMocker.Get<ITrendingGamesRetriever>().AssertWasCalled(
                mock => mock.GetResults(Arg<TrendingGamesRequest>.Matches(
                    x => x.NumberOfTrendingGamesToShow == numberOfGamesToShow
                && x.NumberOfDaysOfTrendingGames == numberOfDaysToConsider)));
            results.ShouldNotBeNull();
            var viewModel = results.Model as List<TrendingGameViewModel>;
            viewModel.ShouldNotBeNull();
            results.ViewName.ShouldBe(MVC.GameDefinition.Views._TrendingGamesPartial);
            viewModel[0].ShouldBeSameAs(expectedViewModel1);
            viewModel[1].ShouldBeSameAs(expectedViewModel2);
        }
    }
}
