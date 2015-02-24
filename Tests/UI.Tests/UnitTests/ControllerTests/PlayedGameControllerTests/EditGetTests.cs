using NUnit.Framework;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
	[TestFixture]
	public class EditGetTests : PlayedGameControllerTestBase
	{
		[Test]
		public void ThatEditActionReturnsAView()
		{
			//--Act
			var result = base.playedGameControllerPartialMock.Edit() as ViewResult;

			//--Assert
			Assert.AreEqual(MVC.PlayedGame.Views.Edit, result.ViewName);
		}
	}
}