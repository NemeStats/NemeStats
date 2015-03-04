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
using UI.Controllers;
using UI.Models.Players;

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

            playerEditViewModelBuilderMock.Expect(mock => mock.Build(Arg<Player>.Is.Anything))
                                          .Return(expectedViewModel);
        }

        [Test]
        public void ItRedirectsToTheGamingGroupIndexAndPlayersSectionAfterSaving()
        {
            string baseUrl = "base url";
            string expectedUrl = baseUrl + "#" + GamingGroupController.SECTION_ANCHOR_PLAYERS;
            urlHelperMock.Expect(mock => mock.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name))
                    .Return(baseUrl);
            Player player = new Player()
            {
                Name = "player name"
            };

            RedirectResult redirectResult = playerController.Edit(player, currentUser) as RedirectResult;

            Assert.AreEqual(expectedUrl, redirectResult.Url);
        }

        [Test]
        public void ItRemainsOnTheEditViewIfValidationFails()
        {
            playerController.ModelState.AddModelError("key", "message");

            ViewResult result = playerController.Edit(new Player(), currentUser) as ViewResult;

            Assert.AreEqual(MVC.Player.Views.Edit, result.ViewName);
        }

        [Test]
        public void ItPutsThePlayerOnTheViewIfValidationFails()
        {
            Player player = new Player();
            playerController.ModelState.AddModelError("key", "message");

            ViewResult result = playerController.Edit(player, currentUser) as ViewResult;

            Assert.AreEqual(expectedViewModel, result.Model);
        }

        [Test]
        public void ItSavesThePlayer()
        {
            Player player = new Player()
            {
                Name = "player name"
            };

            playerController.Edit(player, currentUser);

            playerSaverMock.AssertWasCalled(mock => mock.Save(player, currentUser));
        }
    }
}
