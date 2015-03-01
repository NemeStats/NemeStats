using AutoMapper;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Web.Mvc;
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
			var editedGame = new NewlyCompletedGameViewModel();
			base.playedGameControllerPartialMock.Expect(mock => mock.Edit(Arg<NewlyCompletedGameViewModel>.Is.Anything, Arg<ApplicationUser>.Is.Anything)).Return(new ViewResult { ViewName = MVC.GamingGroup.Views.Index });

			//--Act
			var result = base.playedGameControllerPartialMock.Edit(editedGame, base.currentUser) as ViewResult;

			//--Assert
			Assert.AreEqual(MVC.GamingGroup.Views.Index, result.ViewName);
		}

		[Test]
		public void ItStaysOnEditPageIfValidationFails()
		{
			//--Arrange
			var editedGame = new NewlyCompletedGameViewModel();
			base.playedGameControllerPartialMock.ModelState.AddModelError("Model", "is bad you fool");

			//--Act
			var result = playedGameControllerPartialMock.Edit(editedGame, base.currentUser) as ViewResult;

			//--Assert
			Assert.AreEqual(MVC.PlayedGame.Views.Edit, result.ViewName);
		}

		[Test]
		public void ThatItSavesTheGame()
		{
			//--Arrange
			var editedGame = new NewlyCompletedGame();
			var viewModel = new NewlyCompletedGameViewModel();
			Mapper.Map<NewlyCompletedGameViewModel, NewlyCompletedGame>(viewModel, editedGame);

			//--Act
			base.playedGameController.Edit(viewModel, base.currentUser);

			//--Assert
			base.playedGameCreatorMock.AssertWasCalled(mock => mock.CreatePlayedGame(Arg<NewlyCompletedGame>.Is.Anything, Arg<ApplicationUser>.Is.Anything));
		}

		[Test]
		public void ThatPreviousGameIsDeleted()
		{
			//--Arrange
			var viewModel = new NewlyCompletedGameViewModel();

			//--Act
			base.playedGameControllerPartialMock.Edit(viewModel, base.currentUser);

			//--Assert
			base.playedGameDeleterMock.AssertWasCalled(mock => mock.DeletePlayedGame(Arg<int>.Is.NotNull, Arg<ApplicationUser>.Is.Anything));
		}
	}
}