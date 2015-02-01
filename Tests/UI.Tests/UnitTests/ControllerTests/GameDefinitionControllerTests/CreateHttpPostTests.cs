using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Web.Mvc;
using UI.Controllers;
using UI.Models.GameDefinitionModels;

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
	[TestFixture]
	public class CreateHttpPostTests : GameDefinitionControllerTestBase
	{
		[Test]
		public void ItStaysOnTheCreatePageIfValidationFails()
		{
			gameDefinitionControllerPartialMock.ModelState.AddModelError("key", "message");

			ViewResult viewResult = gameDefinitionControllerPartialMock.Create(null, currentUser) as ViewResult;

			Assert.AreEqual(MVC.GameDefinition.Views.Create, viewResult.ViewName);
		}

		[Test]
		public void ItReloadsTheCurrentGameDefinitionIfValidationFails()
		{
			var newGameDefinitionViewModel = new NewGameDefinitionViewModel();
			gameDefinitionControllerPartialMock.ModelState.AddModelError("key", "message");

			ViewResult actionResult = gameDefinitionControllerPartialMock.Create(newGameDefinitionViewModel, currentUser) as ViewResult;
			var actualViewModel = (NewGameDefinitionViewModel)actionResult.ViewData.Model;

			Assert.AreSame(newGameDefinitionViewModel, actualViewModel);
		}

		[Test]
		public void ItSavesTheGameDefinitionIfValidationPasses()
		{
			var newGameDefinitionViewModel = new NewGameDefinitionViewModel()
			{
				Name = "game definition name"
			};

            gameDefinitionControllerPartialMock.Create(newGameDefinitionViewModel, currentUser);

			gameDefinitionCreatorMock.AssertWasCalled(mock => mock.Save(
                Arg<GameDefinition>.Matches(x => x.Name == newGameDefinitionViewModel.Name
                                            && x.Description == newGameDefinitionViewModel.Description
                                            && x.BoardGameGeekObjectId == newGameDefinitionViewModel.BoardGameGeekObjectId), 
                Arg<ApplicationUser>.Is.Same(currentUser)));
		}

		[Test]
		public void ItRedirectsToTheGamingGroupIndexAndGameDefinitionSectionAfterSaving()
		{
			string baseUrl = "base url";
			string expectedUrl = baseUrl + "#" + GamingGroupController.SECTION_ANCHOR_GAMEDEFINITIONS;
			var newGameDefinitionViewModel = new NewGameDefinitionViewModel()
			{
				Name = "game definition name"
			};
			urlHelperMock.Expect(mock => mock.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name))
					.Return(baseUrl);

			RedirectResult redirectResult = gameDefinitionControllerPartialMock.Create(newGameDefinitionViewModel, currentUser) as RedirectResult;

			Assert.AreEqual(expectedUrl, redirectResult.Url);
		}

		[Test]
		public void ItRedirectsBackToThePlayedGameCreateWhenSentFromThere()
		{
		    int expectedGameDefinitionId = 123;
			string returnUrl = "/PlayedGame/Create";
		    var newGameDefinitionViewModel = new NewGameDefinitionViewModel()
		    {
                ReturnUrl = returnUrl,
                Name = "Project-Ariel"
		    };
		    gameDefinitionCreatorMock.Expect(mock => mock.Save(Arg<GameDefinition>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
		                             .Return(new GameDefinition()
		                             {
		                                 Id = expectedGameDefinitionId
		                             });
            string expectedUrl = returnUrl + "?gameId=" + expectedGameDefinitionId;

            RedirectResult redirectResult = gameDefinitionControllerPartialMock.Create(newGameDefinitionViewModel, currentUser) as RedirectResult;

			Assert.AreEqual(expectedUrl, redirectResult.Url);
		}
	}
}
