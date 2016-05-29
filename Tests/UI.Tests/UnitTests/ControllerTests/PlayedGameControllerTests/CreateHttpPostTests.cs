#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion

using System;
using BusinessLogic.Logic;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UI.Controllers;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
	[TestFixture]
	public class CreateHttpPostTests : PlayedGameControllerTestBase
	{
		[Test]
		public void ItRemainsOnTheCreatePageIfTheModelIsNotValid()
		{
			var expectedViewResult = new ViewResult();
            autoMocker.PartialMockTheClassUnderTest();
			autoMocker.ClassUnderTest.Expect(controller => controller.Create(currentUser))
					.Repeat.Once()
					.Return(expectedViewResult);
			autoMocker.ClassUnderTest.ModelState.AddModelError("Test error", "this is a test error to make model state invalid");

			var actualResult = autoMocker.ClassUnderTest.Create(new NewlyCompletedGame(), false, currentUser) as ViewResult;

			Assert.AreSame(expectedViewResult, actualResult);
		}

        [Test]
        public void ItRemainsOnTheCreatePageAfterLoggingIfTheUserWantsToRecordAnotherGame()
        {
            //--arrange
            var expectedViewResult = new ViewResult();
            autoMocker.PartialMockTheClassUnderTest();
            autoMocker.ClassUnderTest.Expect(controller => controller.Create(currentUser))
                    .Repeat.Once()
                    .Return(expectedViewResult);

            //--act
            var actualResult = autoMocker.ClassUnderTest.Create(new NewlyCompletedGame(), true, currentUser) as ViewResult;

            //--assert
            Assert.AreSame(expectedViewResult, actualResult);
        }

        [Test]
		public void ItRedirectsToTheGamingGroupIndexAndRecentGamesSectionAfterSaving()
		{
			var playedGame = new NewlyCompletedGame()
			{
				GameDefinitionId = 1,
				PlayerRanks = new List<PlayerRank>()
			};
			var baseUrl = "base url";
			var expectedUrl = baseUrl + "#" + GamingGroupController.SECTION_ANCHOR_RECENT_GAMES;
            autoMocker.ClassUnderTest.Url.Expect(mock => mock.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name))
					.Return(baseUrl);

            autoMocker.Get<IPlayedGameCreator>().Expect(x => x.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Is.Anything, 
                Arg<TransactionSource>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything)).Repeat.Once();

			var redirectResult = autoMocker.ClassUnderTest.Create(playedGame, false, null) as RedirectResult;

			Assert.AreEqual(expectedUrl, redirectResult.Url);
		}

		[Test]
		public void ItSavesTheNewGame()
		{
			var newlyCompletedGame = new NewlyCompletedGame()
			{
				GameDefinitionId = 1,
				PlayerRanks = new List<PlayerRank>()
			};
			var baseUrl = "base url";
			autoMocker.ClassUnderTest.Url.Expect(mock => mock.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name))
					.Return(baseUrl);

			autoMocker.ClassUnderTest.Create(newlyCompletedGame, false, null);

			autoMocker.Get<IPlayedGameCreator>().AssertWasCalled(mock => mock.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Is.Equal(newlyCompletedGame),
                Arg<TransactionSource>.Is.Anything,
				Arg<ApplicationUser>.Is.Anything));
		}

		[Test]
		public void ItMakesTheRequestForTheCurrentUser()
		{
			var newlyCompletedGame = new NewlyCompletedGame()
			{
				GameDefinitionId = 1,
				PlayerRanks = new List<PlayerRank>()
			};

			autoMocker.ClassUnderTest.Create(newlyCompletedGame, false, currentUser);

			autoMocker.Get<IPlayedGameCreator>().AssertWasCalled(logic => logic.CreatePlayedGame(
				Arg<NewlyCompletedGame>.Is.Anything,
                Arg<TransactionSource>.Is.Anything,
				Arg<ApplicationUser>.Is.Equal(this.currentUser)));
		}
    }
}
