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

namespace UI.Tests.UnitTests.ControllerTests.GameDefinitionControllerTests
{
    [TestFixture]
    public class EditHttpPostTests : GameDefinitionControllerTestBase
    {
        [Test]
        public void ItStaysOnTheEditPageIfValidationFails()
        {
            GameDefinition gameDefinition = new GameDefinition();
            gameDefinitionControllerPartialMock.ModelState.AddModelError("key", "message");

            ViewResult viewResult = gameDefinitionControllerPartialMock.Edit(gameDefinition, currentUser) as ViewResult;

            Assert.AreEqual(MVC.GameDefinition.Views.Edit, viewResult.ViewName);
        }

        [Test]
        public void ItReloadsTheGameDefinitionIfValidationFails()
        {
            GameDefinition gameDefinition = new GameDefinition();
            gameDefinitionControllerPartialMock.ModelState.AddModelError("key", "message");

            ViewResult viewResult = gameDefinitionControllerPartialMock.Edit(gameDefinition, currentUser) as ViewResult;

            Assert.AreSame(gameDefinition, viewResult.Model);
        }

        [Test]
        public void ItSavesTheGameDefinitionIfValidationPasses()
        {
            GameDefinition gameDefinition = new GameDefinition()
            {
                Name = "game definition name"
            };

            gameDefinitionControllerPartialMock.Edit(gameDefinition, currentUser);

            dataContextMock.AssertWasCalled(mock => mock.Save(gameDefinition, currentUser));
        }

        [Test]
        public void ItRedirectsToTheGamingGroupIndexAndGameDefinitionsSectionAfterSaving()
        {
            GameDefinition gameDefinition = new GameDefinition()
            {
                Name = "game definition name"
            };
            string baseUrl = "base url";
            string expectedUrl = baseUrl + "#" + GamingGroupController.SECTION_ANCHOR_GAMEDEFINITIONS;
            urlHelperMock.Expect(mock => mock.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name))
                    .Return(baseUrl);

            RedirectResult redirectResult = gameDefinitionControllerPartialMock.Edit(gameDefinition, currentUser) as RedirectResult;

            Assert.AreEqual(expectedUrl, redirectResult.Url);
        }
    }
}
