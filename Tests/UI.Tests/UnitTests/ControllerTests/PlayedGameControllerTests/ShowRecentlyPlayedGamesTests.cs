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

using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models.Games;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
	[TestFixture]
	public class ShowRecentlyPlayedGamesTests : PlayedGameControllerTestBase
	{
		[SetUp]
		public override void TestSetUp()
		{
			base.TestSetUp();
			expectedViewModel = new List<PublicGameSummary>();
            autoMocker.Get<IPlayedGameRetriever>().Expect(mock => mock.GetRecentPublicGames(Arg<int>.Is.Anything)).Return(new List<PublicGameSummary>());
            autoMocker.PartialMockTheClassUnderTest();
			autoMocker.ClassUnderTest.Expect(mock => mock.ShowRecentlyPlayedGames()).Return(new ViewResult { ViewName = MVC.PlayedGame.Views.RecentlyPlayedGames, ViewData = new ViewDataDictionary(base.expectedViewModel) });
		}

		[Test]
		public void ItReturnsRecentlyPlayedGamesView()
		{
			var viewResult = autoMocker.ClassUnderTest.ShowRecentlyPlayedGames() as ViewResult;

			Assert.AreEqual(MVC.PlayedGame.Views.RecentlyPlayedGames, viewResult.ViewName);
		}

		[Test]
		public void ItReturnsSpecifiedRecentlyPlayedGamesModelToView()
		{
			var viewResult = autoMocker.ClassUnderTest.ShowRecentlyPlayedGames() as ViewResult;

			var actualViewModel = viewResult.ViewData.Model;

			Assert.AreEqual(base.expectedViewModel, actualViewModel);
		}
	}
}