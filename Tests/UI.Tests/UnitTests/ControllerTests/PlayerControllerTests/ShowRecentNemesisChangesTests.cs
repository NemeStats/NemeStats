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
			base.SetUp();

			var viewResult = playerController.ShowRecentNemesisChanges() as ViewResult;

			Assert.AreEqual(MVC.Player.Views.RecentNemesisChanges, viewResult.ViewName);
		}
	}
}
