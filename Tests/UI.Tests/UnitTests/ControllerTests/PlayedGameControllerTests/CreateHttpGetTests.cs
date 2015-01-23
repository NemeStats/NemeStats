using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UI.Models.PlayedGame;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
	[TestFixture]
	public class CreateHttpGetTests : PlayedGameControllerTestBase
	{
		private int playerId = 1938;
		private string playerName = "Herb";
		private List<Player> allPlayers;

		[SetUp]
		public override void TestSetUp()
		{
			base.TestSetUp();

			allPlayers = new List<Player>() { new Player() { Id = playerId, Name = playerName } };

			playerRetrieverMock.Expect(x => x.GetAllPlayers(currentUser.CurrentGamingGroupId.Value)).Repeat.Once().Return(allPlayers);
		}

		[Test]
		public void ItSetsTheGameDefinitionsOnTheViewModel()
		{
			int gameDefinitionId = 1;
			var gameDefinitions = new List<GameDefinitionSummary>
            {
                new GameDefinitionSummary
                {
                   Id = gameDefinitionId
                }
            };
			base.gameDefinitionRetrieverMock.Expect(mock => mock.GetAllGameDefinitions(currentUser.CurrentGamingGroupId.Value))
				.Return(gameDefinitions);

			ViewResult result = playedGameController.Create(currentUser) as ViewResult;

			NewlyCompletedGameViewModel viewModel = (NewlyCompletedGameViewModel)result.Model;
			Assert.That(viewModel.GameDefinitions.All(item => item.Value == gameDefinitionId.ToString()), Is.True);
		}

		[Test]
		public void ItLoadsTheCreateView()
		{
			ViewResult result = playedGameController.Create(currentUser) as ViewResult;

			Assert.AreEqual(MVC.PlayedGame.Views.Create, result.ViewName);
		}
	}
}
