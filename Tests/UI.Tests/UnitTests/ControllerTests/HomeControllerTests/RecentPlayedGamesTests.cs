using System;
using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic.Facades;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.PlayedGames;
using NemeStats.TestingHelpers.NemeStatsTestingExtensions;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;
using UI.Controllers;

namespace UI.Tests.UnitTests.ControllerTests.HomeControllerTests
{
    public class RecentPlayedGamesTests : HomeControllerTestBase
    {
        [Test]
        public void It_Returns_The_Last_Five_PublicGameSummaries_With_A_Max_Date_Of_Tomorrow_And_Min_Date_Of_2_Days_Ago()
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
            var args = _autoMocker.Get<IRecentPublicGamesRetriever>().GetArgumentsForCallsMadeOn(
                mock => mock.GetResults(Arg<RecentlyPlayedGamesFilter>.Is.Anything));
            var actualFilter = args.AssertFirstCallIsType<RecentlyPlayedGamesFilter>();
            actualFilter.MaxDate.ShouldBe(DateTime.UtcNow.Date.AddDays(1));
            actualFilter.MinDate.ShouldBe(DateTime.UtcNow.Date.AddDays(-2));
            actualFilter.NumberOfGamesToRetrieve.ShouldBe(HomeController.NUMBER_OF_RECENT_PUBLIC_GAMES_TO_SHOW);

            var viewResult = results as PartialViewResult;
            viewResult.ShouldNotBeNull();
            viewResult.ViewName.ShouldBe(MVC.PlayedGame.Views._RecentlyPlayedGamesPartial);
            var viewModel = viewResult.Model as List<PublicGameSummary>;
            viewModel.ShouldNotBeNull();
            viewModel.ShouldBeSameAs(expectedResults);
        }
    }
}
