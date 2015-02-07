using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using UI.Models.Players;

namespace UI.Tests.UnitTests.ControllerTests.PlayerControllerTests
{
	[TestFixture]
	public class ShowTopPlayersTests : PlayerControllerTestBase
	{
		[Test]
		public void ItReturnsTopPlayerView()
		{
			var viewResult = playerController.ShowTopPlayers() as ViewResult;

			Assert.AreEqual(MVC.Player.Views.TopPlayers, viewResult.ViewName);
		}

		[Test]
		public void ItReturnsSpecifiedTopPlayerModelToView()
		{
			var viewResult = playerController.ShowTopPlayers() as ViewResult;

			var actualViewModel = viewResult.ViewData.Model;

			Assert.AreEqual(base.expectedTopPlayersViewModel, actualViewModel);
		}
	}
}