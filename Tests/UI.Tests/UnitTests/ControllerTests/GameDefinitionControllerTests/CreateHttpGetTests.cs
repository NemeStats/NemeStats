using NUnit.Framework;
using System.Web.Mvc;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
	[TestFixture]
	public class CreateHttpGetTests : GameDefinitionControllerTestBase
	{
		[Test]
		public void ItReturnsACreateView()
		{
			ViewResult viewResult = gameDefinitionControllerPartialMock.Create(string.Empty) as ViewResult;

			Assert.AreEqual(MVC.GameDefinition.Views.Create, viewResult.ViewName);
		}
	}
}
