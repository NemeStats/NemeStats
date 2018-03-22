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
using BusinessLogic.Models;
using NUnit.Framework;
using Rhino.Mocks;
using System.Linq;
using System.Web.Mvc;
using BusinessLogic.Exceptions;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using UI.Controllers;
using UI.Models.Players;
using BusinessLogic.Logic.Players;
using Shouldly;
using UI.Transformations.PlayerTransformations;

namespace UI.Tests.UnitTests.ControllerTests.PlayerControllerTests
{
    [TestFixture]
    public class EditHttpPostTests : PlayerControllerTestBase
    {
        private readonly PlayerEditViewModel expectedViewModel = new PlayerEditViewModel();

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            autoMocker.Get<IPlayerEditViewModelBuilder>().Expect(mock => mock.Build(Arg<Player>.Is.Anything))
                                          .Return(expectedViewModel);
        }

        [Test]
        public void ItRedirectsToTheGamingGroupIndexAndPlayersSectionAfterSaving()
        {
            var baseUrl = "base url";
            var expectedUrl = baseUrl + "#" + GamingGroupController.SECTION_ANCHOR_PLAYERS;
            autoMocker.ClassUnderTest.Url.Expect(mock => mock.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name))
                    .Return(baseUrl);
            var player = new Player()
            {
                Name = "player name"
            };

            var redirectResult = autoMocker.ClassUnderTest.Edit(player, currentUser) as RedirectResult;

            Assert.AreEqual(expectedUrl, redirectResult.Url);
        }

        [Test]
        public void ItRemainsOnTheEditViewAndKeepsThePlayerIfValidationFails()
        {
            autoMocker.ClassUnderTest.ModelState.AddModelError("key", "message");

            var result = autoMocker.ClassUnderTest.Edit(new Player(), currentUser) as ViewResult;

            Assert.AreEqual(MVC.Player.Views.Edit, result.ViewName);
            Assert.AreEqual(expectedViewModel, result.Model);
        }

        [Test]
        public void ItStaysOnTheSameViewAndGivesAValidationErrorIfThePlayerNameIsAlreadyTaken()
        {
            //--arrange
            var expectedException = new PlayerAlreadyExistsException("some name", 1);
            autoMocker.Get<IPlayerSaver>().Expect(mock =>
                    mock.UpdatePlayer(Arg<UpdatePlayerRequest>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Throw(expectedException);
            var player = new Player
            {
                Name = "bubba"
            };

            //--act
            var result = autoMocker.ClassUnderTest.Edit(player, currentUser) as ViewResult;

            //--assert
            Assert.AreEqual(MVC.Player.Views.Edit, result.ViewName);
            Assert.AreEqual(expectedViewModel, result.Model);
            autoMocker.ClassUnderTest.ModelState.ContainsKey(string.Empty).ShouldBeTrue();
            var modelErrorsForKey = autoMocker.ClassUnderTest.ModelState[string.Empty].Errors;
            modelErrorsForKey.Count.ShouldBe(1);
            modelErrorsForKey[0].ErrorMessage.ShouldBe($"A Player with name '{player.Name}' already exists in this Gaming Group. Choose another.");
        }

        [Test]
        public void ItSavesThePlayer()
        {
            var player = new Player()
            {
                Name = "player name"
            };

            autoMocker.ClassUnderTest.Edit(player, currentUser);

            autoMocker.Get<IPlayerSaver>().AssertWasCalled(mock => mock.UpdatePlayer(
             Arg<UpdatePlayerRequest>.Matches(p => p.Active == player.Active
                                 && p.Name == player.Name
                                 && p.PlayerId == player.Id),
             Arg<ApplicationUser>.Is.Same(currentUser)));
        }
    }
}
