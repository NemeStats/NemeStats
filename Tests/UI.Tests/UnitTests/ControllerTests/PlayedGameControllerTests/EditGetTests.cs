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

			base.PlayerList = new List<Player> { new Player { Id = 42, Name = "Smitty Werbenjagermanjensen" } };
			base.PlayerSelectList = this.PlayerList.Select(item => new SelectListItem { Text = item.Name, Value = item.Id.ToString() }).ToList();
			base.GameDefinitionList = new List<GameDefinition> { new GameDefinition { Id = 1, Name = "Betrayal At The House On The Hill" } };
			base.GameDefinitionSelectList = this.GameDefinitionList.Select(item => new SelectListItem { Text = item.Name, Value = item.Id.ToString() }).ToList();
			base.ExpectedPopulatedCompletedGameViewModel = new PlayedGameEditViewModel { GameDefinitions = base.GameDefinitionSelectList, Players = base.PlayerSelectList };
		}

		[Test]
		public void ThatEditGetActionReturnsAView()
		{
			//--Arrange
            AutoMocker.PartialMockTheClassUnderTest();
			AutoMocker.ClassUnderTest.Expect(mock => mock.Edit()).Return(new ViewResult { ViewName = MVC.PlayedGame.Views.Edit });

			//--Act
			var result = AutoMocker.ClassUnderTest.Edit() as ViewResult;

			//--Assert
			Assert.AreEqual(MVC.PlayedGame.Views.Edit, result.ViewName);
		}

		[Test]
		public void ThatWhenGameIDIsNotZeroPopulatedModelIsSentToView()
		{
			//--Arrange
            AutoMocker.PartialMockTheClassUnderTest();
			AutoMocker.ClassUnderTest.Expect(mock => mock.Edit(Arg<int>.Is.Anything, Arg<ApplicationUser>.Is.Anything)).Return(new ViewResult { ViewData = new ViewDataDictionary(base.ExpectedPopulatedCompletedGameViewModel) });

			//--Act
			var result = AutoMocker.ClassUnderTest.Edit(1, base.CurrentUser) as ViewResult;

			//--Assert
			Assert.AreEqual(base.ExpectedPopulatedCompletedGameViewModel, result.ViewData.Model);
		}
	}
}