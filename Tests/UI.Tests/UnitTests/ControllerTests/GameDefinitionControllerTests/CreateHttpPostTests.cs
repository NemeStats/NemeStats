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
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Web.Mvc;
using BusinessLogic.Logic.GameDefinitions;
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
			autoMocker.ClassUnderTest.ModelState.AddModelError("key", "message");

			ViewResult viewResult = autoMocker.ClassUnderTest.Create(null, currentUser) as ViewResult;

			Assert.AreEqual(MVC.GameDefinition.Views.Create, viewResult.ViewName);
		}

		[Test]
		public void ItReloadsTheCurrentGameDefinitionIfValidationFails()
		{
			var createGameDefinitionRequest = new CreateGameDefinitionViewModel();
			autoMocker.ClassUnderTest.ModelState.AddModelError("key", "message");

			ViewResult actionResult = autoMocker.ClassUnderTest.Create(createGameDefinitionRequest, currentUser) as ViewResult;
			var actualViewModel = (CreateGameDefinitionViewModel)actionResult.ViewData.Model;

			Assert.AreSame(createGameDefinitionRequest, actualViewModel);
		}

		[Test]
		public void ItSavesTheGameDefinitionIfValidationPasses()
		{
			var createGameDefinitionRequest = new CreateGameDefinitionViewModel()
			{
				Name = "game definition name"
			};

            autoMocker.ClassUnderTest.Create(createGameDefinitionRequest, currentUser);

            autoMocker.Get<ICreateGameDefinitionComponent>().AssertWasCalled(mock => mock.Execute(
                Arg<CreateGameDefinitionRequest>.Matches(x => x.Name == createGameDefinitionRequest.Name
                                            && x.Description == createGameDefinitionRequest.Description
                                            && x.BoardGameGeekGameDefinitionId == createGameDefinitionRequest.BoardGameGeekGameDefinitionId), 
                Arg<ApplicationUser>.Is.Same(currentUser)));
		}

		[Test]
		public void ItRedirectsToTheGamingGroupIndexAndGameDefinitionSectionAfterSaving()
		{
			string baseUrl = "base url";
			string expectedUrl = baseUrl + "#" + GamingGroupController.SECTION_ANCHOR_GAMEDEFINITIONS;
			var createGameDefinitionRequest = new CreateGameDefinitionViewModel()
			{
				Name = "game definition name"
			};
			autoMocker.ClassUnderTest.Url.Expect(mock => mock.Action(MVC.GamingGroup.ActionNames.Index, MVC.GamingGroup.Name))
					.Return(baseUrl);

			RedirectResult redirectResult = autoMocker.ClassUnderTest.Create(createGameDefinitionRequest, currentUser) as RedirectResult;

			Assert.AreEqual(expectedUrl, redirectResult.Url);
		}

		[Test]
		public void ItRedirectsBackToThePlayedGameCreateWhenSentFromThere()
		{
		    int expectedGameDefinitionId = 123;
			string returnUrl = "/PlayedGame/Create";
		    var createGameDefinitionRequest = new CreateGameDefinitionViewModel()
		    {
                ReturnUrl = returnUrl,
                Name = "Project-Ariel"
		    };
		    autoMocker.Get<ICreateGameDefinitionComponent>().Expect(mock => mock.Execute(Arg<CreateGameDefinitionRequest>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
		                             .Return(new GameDefinition 
		                             {
		                                 Id = expectedGameDefinitionId
		                             });
            string expectedUrl = returnUrl + "?gameId=" + expectedGameDefinitionId;

            RedirectResult redirectResult = autoMocker.ClassUnderTest.Create(createGameDefinitionRequest, currentUser) as RedirectResult;

			Assert.AreEqual(expectedUrl, redirectResult.Url);
		}
	}
}
