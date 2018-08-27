#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion

using System;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models.Games;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic.Models.PlayedGames;
using NemeStats.TestingHelpers.NemeStatsTestingExtensions;
using Shouldly;
using UI.Controllers;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
	[TestFixture]
	public class ShowRecentlyPlayedGamesTests : PlayedGameControllerTestBase
	{
		[SetUp]
		public override void TestSetUp()
		{
			base.TestSetUp();
			ExpectedViewModel = new List<PublicGameSummary>();
            AutoMocker.Get<IPlayedGameRetriever>().Expect(mock => mock.GetRecentPublicGames(Arg<RecentlyPlayedGamesFilter>.Is.Anything)).Return(new List<PublicGameSummary>());
        }

		[Test]
		public void It_Returns_The_RecentlyPlayedGames_View()
		{
			var viewResult = AutoMocker.ClassUnderTest.ShowRecentlyPlayedGames() as ViewResult;

			viewResult.ViewName.ShouldBe(MVC.PlayedGame.Views.RecentlyPlayedGames);
            var actualViewModel = viewResult.ViewData.Model;
            actualViewModel.ShouldBe(ExpectedViewModel);
        }

		[Test]
		public void It_Returns_Recently_Played_Games_That_Have_Happened_Less_Than_Or_Equal_To_UTC_Today_To_Avoid_Thai_Buddhist_Future_Dated_Plays()
		{
			AutoMocker.ClassUnderTest.ShowRecentlyPlayedGames();

            var args =
		        AutoMocker.Get<IPlayedGameRetriever>()
		            .GetArgumentsForCallsMadeOn(mock => mock.GetRecentPublicGames(Arg<RecentlyPlayedGamesFilter>.Is.Anything));
		    var firstCall = args.AssertFirstCallIsType<RecentlyPlayedGamesFilter>();
		    firstCall.MaxDate.ShouldBe(DateTime.UtcNow.Date.AddDays(1));
            firstCall.NumberOfGamesToRetrieve.ShouldBe(PlayedGameController.NUMBER_OF_RECENT_GAMES_TO_DISPLAY);
		}

	    [Test]
	    public void It_Returns_Recently_Played_Games_No_More_Than_7_Days_Old()
	    {
	        AutoMocker.ClassUnderTest.ShowRecentlyPlayedGames();

	        var args =
	            AutoMocker.Get<IPlayedGameRetriever>()
	                .GetArgumentsForCallsMadeOn(mock => mock.GetRecentPublicGames(Arg<RecentlyPlayedGamesFilter>.Is.Anything));
	        var firstCall = args.AssertFirstCallIsType<RecentlyPlayedGamesFilter>();
	        firstCall.MinDate.ShouldBe(DateTime.UtcNow.Date.AddDays(-7));
	    }
    }
}