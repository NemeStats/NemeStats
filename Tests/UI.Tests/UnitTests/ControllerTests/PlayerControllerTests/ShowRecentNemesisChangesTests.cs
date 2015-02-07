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
	public class ShowRecentNemesisChangesTests : PlayerControllerTestBase
	{
		[Test]
		public void ItReturnsRecentNemesisChangesView()
		{
			var viewResult = playerController.ShowRecentNemesisChanges() as ViewResult;

			Assert.AreEqual(MVC.Player.Views.RecentNemesisChanges, viewResult.ViewName);
		}

		[Test]
		public void ItReturnsSpecifiedRecentNemesisChangeModelToView()
		{
			var viewResult = playerController.ShowRecentNemesisChanges() as ViewResult;

			var actualViewModel = viewResult.ViewData.Model;

			Assert.AreEqual(base.expectedNemesisChangeViewModel, actualViewModel);
		}
	}
}
