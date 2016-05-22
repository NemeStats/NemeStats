using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic.Facades;
using BusinessLogic.Models.Games;
using UI.Controllers;
using UI.Models.GameDefinitionModels;
using UI.Transformations;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
    [TestFixture]
    public class ShowTrendingGamesTests : GameDefinitionControllerTestBase
    {
        [Test]
        public void ItReturnsTrendingGamesView()
        {
            //--arrange
            autoMocker.Get<ITrendingGamesRetriever>()
                      .Expect(mock => mock.GetResults(null)).IgnoreArguments().Return(new List<TrendingGame>());
           
           //--act
           var viewResult = autoMocker.ClassUnderTest.ShowTrendingGames() as ViewResult;

            //--assert
            Assert.AreEqual(MVC.GameDefinition.Views.TrendingGames, viewResult.ViewName);
        }

        [Test]
        public void ItReturnsTheSpecifiedNumberOfTrendingGames()
        {
            var expectedTrendingGame = new TrendingGame();
            var expectedTrendingGames = new List<TrendingGame>
            {
                expectedTrendingGame
            };

            autoMocker.Get<ITrendingGamesRetriever>()
                      .Expect(
                              mock =>
                              mock.GetResults(Arg<TrendingGamesRequest>.Matches(x => x.NumberOfDaysOfTrendingGames == GameDefinitionController.NUMBER_OF_DAYS_OF_TRENDING_GAMES
                                                                                     &&
                                                                                     x.NumberOfTrendingGamesToShow == GameDefinitionController.NUMBER_OF_TRENDING_GAMES_TO_SHOW)))
                      .Return(expectedTrendingGames);

            var expectedViewModel = new TrendingGameViewModel();
            autoMocker.Get<ITransformer>().Expect(mock => mock.Transform<TrendingGame, TrendingGameViewModel>(
                expectedTrendingGame))
                .Return(expectedViewModel);
            var viewResult = autoMocker.ClassUnderTest.ShowTrendingGames() as ViewResult;

            var actualModel = viewResult.ViewData.Model as List<TrendingGameViewModel>;

            Assert.That(actualModel[0], Is.SameAs(expectedViewModel));
        }
    }
}