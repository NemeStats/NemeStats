using System;
using AutoMapper;
using BusinessLogic.Logic;
using BusinessLogic.Logic.PlayedGames;
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
            autoMocker.PartialMockTheClassUnderTest();
            autoMocker.ClassUnderTest.Expect(mock => mock.Edit(Arg<PlayedGameEditViewModel>.Is.Anything, Arg<int>.Is.NotNull, Arg<ApplicationUser>.Is.Anything)).Return(new ViewResult { ViewName = MVC.GamingGroup.Views.Index });

			//--Act
			var result = autoMocker.ClassUnderTest.Edit(editedGame, 1234, base.currentUser) as ViewResult;

			//--Assert
			Assert.AreEqual(MVC.GamingGroup.Views.Index, result.ViewName);
		}

		[Test]
		public void ItStaysOnEditPageIfValidationFails()
		{
			//--Arrange
			var editedGame = new PlayedGameEditViewModel();
            autoMocker.PartialMockTheClassUnderTest();
			autoMocker.ClassUnderTest.ModelState.AddModelError("Model", "is bad you fool");

			//--Act
			var result = autoMocker.ClassUnderTest.Edit(editedGame, 1234, base.currentUser) as RedirectToRouteResult;

			//--Assert
			Assert.AreEqual("Edit", result.RouteValues["action"]);
		}

		[Test]
		public void ThatItSavesTheGame()
		{
			//--Arrange
			var editedGame = new NewlyCompletedGame();
			var viewModel = new PlayedGameEditViewModel();
			Mapper.Map(viewModel, editedGame);

			//--Act
			autoMocker.ClassUnderTest.Edit(viewModel, 1234, currentUser);

			//--Assert
            autoMocker.Get<IPlayedGameCreator>().AssertWasCalled(mock => mock.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Is.Anything, 
                Arg<TransactionSource>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
		}

		[Test]
		public void ThatPreviousGameIsDeleted()
		{
			//--Arrange
			var viewModel = new PlayedGameEditViewModel();
            autoMocker.ClassUnderTest.Url.Expect(mock => mock.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name))
                    .Return("some url");

			//--Act
			autoMocker.ClassUnderTest.Edit(viewModel, 1234, base.currentUser);

			//--Assert
            autoMocker.Get<IPlayedGameDeleter>().AssertWasCalled(mock => mock.DeletePlayedGame(Arg<int>.Is.NotNull, Arg<ApplicationUser>.Is.Anything));
		}

		[Test]
		public void ItRedirectsToTheGamingGroupIndexAndRecentGamesSectionAfterSaving()
		{
			//--Arrange
			var playedGame = new NewlyCompletedGame();

			var baseUrl = "base url";
			var expectedUrl = baseUrl + "#" + GamingGroupController.SECTION_ANCHOR_RECENT_GAMES;
            autoMocker.ClassUnderTest.Url.Expect(mock => mock.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name))
					.Return(baseUrl);

            autoMocker.Get<IPlayedGameCreator>().Expect(x => x.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Is.Anything, 
                Arg<TransactionSource>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything)).Repeat.Once();

			//--Act
			RedirectResult redirectResult = autoMocker.ClassUnderTest.Edit(playedGame, 1234, base.currentUser) as RedirectResult;

			//--Assert
			Assert.AreEqual(expectedUrl, redirectResult.Url);
		}
	}
}