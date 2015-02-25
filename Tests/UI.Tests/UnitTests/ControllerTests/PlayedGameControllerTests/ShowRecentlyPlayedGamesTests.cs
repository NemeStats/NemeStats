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
			base.expectedViewModel = new List<PublicGameSummary>();
			base.playedGameRetriever.Expect(mock => mock.GetRecentPublicGames(Arg<int>.Is.Anything)).Return(new List<PublicGameSummary>());
			base.playedGameControllerPartialMock.Expect(mock => mock.ShowRecentlyPlayedGames()).Return(new ViewResult { ViewName = MVC.PlayedGame.Views.RecentlyPlayedGames, ViewData = new ViewDataDictionary(base.expectedViewModel) });
		}

		[Test]
		public void ItReturnsRecentlyPlayedGamesView()
		{
			var viewResult = playedGameControllerPartialMock.ShowRecentlyPlayedGames() as ViewResult;

			Assert.AreEqual(MVC.PlayedGame.Views.RecentlyPlayedGames, viewResult.ViewName);
		}

		[Test]
		public void ItReturnsSpecifiedRecentlyPlayedGamesModelToView()
		{
			var viewResult = playedGameController.ShowRecentlyPlayedGames() as ViewResult;

			var actualViewModel = viewResult.ViewData.Model;

			Assert.AreEqual(base.expectedViewModel, actualViewModel);
		}
	}
}