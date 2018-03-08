using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using BusinessLogic.Facades;
using BusinessLogic.Logic;
using BusinessLogic.Models.Games;
using Shouldly;
using UI.Models.GameDefinitionModels;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
    [TestFixture]
    public class GetTrendingGamesViewModelsTests : GameDefinitionControllerTestBase
    {
        [Test]
        public void ItReturnsTheSpecifiedNumberOfTrendingGames()
        {
            var expectedTrendingGame1 = new TrendingGame();
            var expectedTrendingGame2 = new TrendingGame();
            var expectedTrendingGames = new List<TrendingGame>
            {
                expectedTrendingGame1,
                expectedTrendingGame2
            };

            autoMocker.Get<ITrendingGamesRetriever>()
                      .Expect(
                              mock =>
                              mock.GetResults(Arg<TrendingGamesRequest>.Is.Anything))
                      .Return(expectedTrendingGames);

            var expectedViewModel1 = new TrendingGameViewModel();
            autoMocker.Get<ITransformer>().Expect(mock => mock.Transform<TrendingGameViewModel>(
                expectedTrendingGame1))
                .Return(expectedViewModel1);

            var expectedViewModel2 = new TrendingGameViewModel();
            autoMocker.Get<ITransformer>().Expect(mock => mock.Transform<TrendingGameViewModel>(
                    expectedTrendingGame2))
                .Return(expectedViewModel2);

            var numberOfGames = 1;
            var numberOfDays = 2;

            //--act
            var results = autoMocker.ClassUnderTest.GetTrendingGamesViewModels(numberOfGames, numberOfDays);

            //--assert
            autoMocker.Get<ITrendingGamesRetriever>()
                .AssertWasCalled(
                    mock =>
                        mock.GetResults(Arg<TrendingGamesRequest>.Matches(x => x.NumberOfDaysOfTrendingGames == numberOfDays
                                                                               &&
                                                                               x.NumberOfTrendingGamesToShow == numberOfGames)));

            results[0].ShouldBeSameAs(expectedViewModel1);
            results[1].ShouldBeSameAs(expectedViewModel2);
        }
    }
}