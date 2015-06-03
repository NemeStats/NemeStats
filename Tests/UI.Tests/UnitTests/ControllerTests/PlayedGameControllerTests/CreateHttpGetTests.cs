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

using BusinessLogic.Logic.GameDefinitions;
using BusinessLogic.Logic.Players;
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

            autoMocker.Get<IPlayerRetriever>().Expect(x => x.GetAllPlayers(currentUser.CurrentGamingGroupId.Value)).Repeat.Once().Return(allPlayers);
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
			autoMocker.Get<IGameDefinitionRetriever>().Expect(mock => mock.GetAllGameDefinitions(currentUser.CurrentGamingGroupId.Value))
				.Return(gameDefinitions);

			ViewResult result = autoMocker.ClassUnderTest.Create(currentUser) as ViewResult;

			PlayedGameEditViewModel viewModel = (PlayedGameEditViewModel)result.Model;
			Assert.That(viewModel.GameDefinitions.All(item => item.Value == gameDefinitionId.ToString()), Is.True);
		}

		[Test]
		public void ItLoadsTheCreateView()
		{
			ViewResult result = autoMocker.ClassUnderTest.Create(currentUser) as ViewResult;

			Assert.AreEqual(MVC.PlayedGame.Views.Create, result.ViewName);
		}
	}
}
