using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.PlayerControllerTests
{
	[TestFixture]
	public class ShowTopPlayersTests : PlayerControllerTestBase
	{
		[Test]
		public void ItReturnsTopPlayerView()
		{
			base.SetUp();

			var viewResult = playerController.ShowTopPlayers() as ViewResult;

			Assert.AreEqual(MVC.Player.Views.TopPlayers, viewResult.ViewName);
		}
	}
}
