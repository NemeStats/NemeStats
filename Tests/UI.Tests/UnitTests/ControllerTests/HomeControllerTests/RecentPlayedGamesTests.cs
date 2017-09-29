using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic.Facades;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.PlayedGames;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using UI.Controllers;

namespace UI.Tests.UnitTests.ControllerTests.HomeControllerTests
{
    public class RecentPlayedGamesTests : HomeControllerTestBase
    {
        [Test]
        public void It_Returns_The_Last_Five_PublicGameSummaries_With_A_Max_Date_Of_Tomorrow()
        {
            //--arrange
            var expectedResults = new List<PublicGameSummary>
            {
                new PublicGameSummary(),
                new PublicGameSummary()
            };
            _autoMocker.Get<IRecentPublicGamesRetriever>()
                .Expect(mock => mock.GetResults(Arg<RecentlyPlayedGamesFilter>.Is.Anything))
                .Return(expectedResults);

            //--act
            var results = _autoMocker.ClassUnderTest.RecentPlayedGames();

            //--assert
            _autoMocker.Get<IRecentPublicGamesRetriever>().AssertWasCalled(
                mock => mock.GetResults(Arg<RecentlyPlayedGamesFilter>.Matches(
                    x => x.NumberOfGamesToRetrieve == HomeController.NUMBER_OF_RECENT_PUBLIC_GAMES_TO_SHOW
                    && x.MaxDate == DateTime.UtcNow.Date.AddDays(1))));
            var viewResult = results as PartialViewResult;
            viewResult.ShouldNotBeNull();
            viewResult.ViewName.ShouldBe(MVC.PlayedGame.Views._RecentlyPlayedGamesPartial);
            var viewModel = viewResult.Model as List<PublicGameSummary>;
            viewModel.ShouldNotBeNull();
            viewModel.ShouldBeSameAs(expectedResults);
        }

    }
}
