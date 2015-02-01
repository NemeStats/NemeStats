using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
	[TestFixture]
	public class ShowRecentlyPlayedGamesTests : PlayedGameControllerTestBase
	{
		[Test]
		public void ItReturnsRecentlyPlayedGamesView()
		{
			base.TestSetUp();

			var viewResult = playedGameControllerPartialMock.ShowRecentlyPlayedGames() as ViewResult;

			Assert.AreEqual(MVC.PlayedGame.Views.RecentlyPlayedGames, viewResult.ViewName);
		}
	}
}
