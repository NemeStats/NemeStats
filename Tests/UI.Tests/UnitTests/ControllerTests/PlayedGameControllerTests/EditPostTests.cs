using AutoMapper;
using BusinessLogic.Logic;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Web.Mvc;
using UI.Controllers;
using UI.Models.PlayedGame;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
	[TestFixture]
	public class EditPostTests : PlayedGameControllerTestBase
	{
		[Test]
		public void ThatEditPostActionReturnsAView()
		{
			//--Arrange
			var editedGame = new PlayedGameEditViewModel();
			base.playedGameControllerPartialMock.Expect(mock => mock.Edit(Arg<PlayedGameEditViewModel>.Is.Anything, Arg<int>.Is.NotNull, Arg<ApplicationUser>.Is.Anything)).Return(new ViewResult { ViewName = MVC.GamingGroup.Views.Index });

			//--Act
			var result = base.playedGameControllerPartialMock.Edit(editedGame, 1234, base.currentUser) as ViewResult;

			//--Assert
			Assert.AreEqual(MVC.GamingGroup.Views.Index, result.ViewName);
		}

		[Test]
		public void ItStaysOnEditPageIfValidationFails()
		{
			//--Arrange
			var editedGame = new PlayedGameEditViewModel();
			base.playedGameControllerPartialMock.ModelState.AddModelError("Model", "is bad you fool");

			//--Act
			var result = base.playedGameControllerPartialMock.Edit(editedGame, 1234, base.currentUser) as RedirectToRouteResult;

			//--Assert
			Assert.AreEqual("Edit", result.RouteValues["action"]);
		}

		[Test]
		public void ThatItSavesTheGame()
		{
			//--Arrange
			var editedGame = new NewlyCompletedGame();
			var viewModel = new PlayedGameEditViewModel();
			Mapper.Map<PlayedGameEditViewModel, NewlyCompletedGame>(viewModel, editedGame);

			//--Act
			base.playedGameController.Edit(viewModel, 1234, base.currentUser);

			//--Assert
			base.playedGameCreatorMock.AssertWasCalled(mock => mock.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Is.Anything, 
                Arg<TransactionSource>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
		}

		[Test]
		public void ThatPreviousGameIsDeleted()
		{
			//--Arrange
			var viewModel = new PlayedGameEditViewModel();

			//--Act
			base.playedGameControllerPartialMock.Edit(viewModel, 1234, base.currentUser);

			//--Assert
			base.playedGameDeleterMock.AssertWasCalled(mock => mock.DeletePlayedGame(Arg<int>.Is.NotNull, Arg<ApplicationUser>.Is.Anything));
		}

		[Test]
		public void ItRedirectsToTheGamingGroupIndexAndRecentGamesSectionAfterSaving()
		{
			//--Arrange
			var playedGame = new NewlyCompletedGame();

			var baseUrl = "base url";
			var expectedUrl = baseUrl + "#" + GamingGroupController.SECTION_ANCHOR_RECENT_GAMES;
			base.urlHelperMock.Expect(mock => mock.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name))
					.Return(baseUrl);

			base.playedGameCreatorMock.Expect(x => x.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Is.Anything, 
                Arg<TransactionSource>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything)).Repeat.Once();

			//--Act
			RedirectResult redirectResult = playedGameController.Edit(playedGame, 1234, base.currentUser) as RedirectResult;

			//--Assert
			Assert.AreEqual(expectedUrl, redirectResult.Url);
		}
	}
}