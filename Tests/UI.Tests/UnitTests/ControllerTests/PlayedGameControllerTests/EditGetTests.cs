using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UI.Models.PlayedGame;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
	[TestFixture]
	public class EditGetTests : PlayedGameControllerTestBase
	{
		[SetUp]
		public override void TestSetUp()
		{
			base.TestSetUp();

			base.playerList = new List<Player> { new Player { Id = 42, Name = "Smitty Werbenjagermanjensen" } };
			base.playerSelectList = this.playerList.Select(item => new SelectListItem { Text = item.Name, Value = item.Id.ToString() }).ToList();
			base.gameDefinitionList = new List<GameDefinition> { new GameDefinition { Id = 1, Name = "Betrayal At The House On The Hill" } };
			base.gameDefinitionSelectList = this.gameDefinitionList.Select(item => new SelectListItem { Text = item.Name, Value = item.Id.ToString() }).ToList();
			base.expectedPopulatedCompletedGameViewModel = new PlayedGameEditViewModel { GameDefinitions = base.gameDefinitionSelectList, Players = base.playerSelectList };
		}

		[Test]
		public void ThatEditGetActionReturnsAView()
		{
			//--Arrange
            autoMocker.PartialMockTheClassUnderTest();
			autoMocker.ClassUnderTest.Expect(mock => mock.Edit()).Return(new ViewResult { ViewName = MVC.PlayedGame.Views.Edit });

			//--Act
			var result = autoMocker.ClassUnderTest.Edit() as ViewResult;

			//--Assert
			Assert.AreEqual(MVC.PlayedGame.Views.Edit, result.ViewName);
		}

		[Test]
		public void ThatWhenGameIDIsNotZeroPopulatedModelIsSentToView()
		{
			//--Arrange
            autoMocker.PartialMockTheClassUnderTest();
			autoMocker.ClassUnderTest.Expect(mock => mock.Edit(Arg<int>.Is.Anything, Arg<ApplicationUser>.Is.Anything)).Return(new ViewResult { ViewData = new ViewDataDictionary(base.expectedPopulatedCompletedGameViewModel) });

			//--Act
			var result = autoMocker.ClassUnderTest.Edit(1, base.currentUser) as ViewResult;

			//--Assert
			Assert.AreEqual(base.expectedPopulatedCompletedGameViewModel, result.ViewData.Model);
		}
	}
}