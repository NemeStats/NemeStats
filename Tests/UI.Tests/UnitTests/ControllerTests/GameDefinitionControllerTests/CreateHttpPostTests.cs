using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;
using System.Web.Mvc;
using UI.Controllers;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
	[TestFixture]
	public class CreateHttpPostTests : GameDefinitionControllerTestBase
	{
		[Test]
		public void ItStaysOnTheCreatePageIfValidationFails()
		{
			gameDefinitionControllerPartialMock.ModelState.AddModelError("key", "message");

			ViewResult viewResult = gameDefinitionControllerPartialMock.Create(null, string.Empty, currentUser) as ViewResult;

			Assert.AreEqual(MVC.GameDefinition.Views.Create, viewResult.ViewName);
		}

		[Test]
		public void ItReloadsTheCurrentGameDefinitionIfValidationFails()
		{
			GameDefinition gameDefinition = new GameDefinition();
			gameDefinitionControllerPartialMock.ModelState.AddModelError("key", "message");

			ViewResult actionResult = gameDefinitionControllerPartialMock.Create(gameDefinition, string.Empty, currentUser) as ViewResult;
			GameDefinition actualViewModel = (GameDefinition)actionResult.ViewData.Model;

			Assert.AreSame(gameDefinition, actualViewModel);
		}

		[Test]
		public void ItSavesTheGameDefinitionIfValidationPasses()
		{
			GameDefinition gameDefinition = new GameDefinition()
			{
				Name = "game definition name"
			};

			gameDefinitionControllerPartialMock.Create(gameDefinition, string.Empty, currentUser);

			gameDefinitionCreatorMock.AssertWasCalled(mock => mock.Save(gameDefinition, currentUser));
		}

		[Test]
		public void ItRedirectsToTheGamingGroupIndexAndGameDefinitionSectionAfterSaving()
		{
			string baseUrl = "base url";
			string expectedUrl = baseUrl + "#" + GamingGroupController.SECTION_ANCHOR_GAMEDEFINITIONS;
			GameDefinition gameDefinition = new GameDefinition()
			{
				Name = "game definition name"
			};
			urlHelperMock.Expect(mock => mock.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name))
					.Return(baseUrl);

			RedirectResult redirectResult = gameDefinitionControllerPartialMock.Create(gameDefinition, string.Empty, currentUser) as RedirectResult;

			Assert.AreEqual(expectedUrl, redirectResult.Url);
		}
	}
}
