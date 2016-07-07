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
using BusinessLogic.Models.Games;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic.Models.Players;
using BusinessLogic.Paging;
using PagedList;
using UI.Models.PlayedGame;

namespace UI.Tests.UnitTests.ControllerTests.PlayedGameControllerTests
{
    [TestFixture]
    public class CreateHttpGetTests : PlayedGameControllerTestBase
    {
        private readonly int playerId = 1938;
        private readonly string playerName = "Herb";
        private PlayersToCreateModel players;


        [SetUp]
        public override void TestSetUp()
        {
            base.TestSetUp();

            players = new PlayersToCreateModel { UserPlayer = new PlayerInfoForUser() { PlayerId = playerId }, OtherPlayers = new List<PlayerInfoForUser>(), RecentPlayers = new List<PlayerInfoForUser>() };

            autoMocker.Get<IGameDefinitionRetriever>().Expect(x => x.GetMostPlayedGames(Arg<GetMostPlayedGamesQuery>.Is.Anything)).Repeat.Once().Return(new PagedList<GameDefinitionDisplayInfo>(new List<GameDefinitionDisplayInfo>(), 1, 1));
            autoMocker.Get<IGameDefinitionRetriever>().Expect(x => x.GetRecentGames(Arg<GetRecentPlayedGamesQuery>.Is.Anything)).Repeat.Once().Return(new PagedList<GameDefinitionDisplayInfo>(new List<GameDefinitionDisplayInfo>(), 1, 1));
            autoMocker.Get<IPlayerRetriever>().Expect(x => x.GetPlayersToCreate(currentUser.Id, currentUser.CurrentGamingGroupId)).Repeat.Once().Return(players);

        }


        [Test]
        public void ItLoadsTheCreateOrEditView()
        {
            var result = autoMocker.ClassUnderTest.Create(currentUser) as ViewResult;

            Assert.AreEqual(MVC.PlayedGame.Views.CreateOrEdit, result.ViewName);
        }

        [Test]
        public void ItReturnsAFilledCreatePlayedGameViewModel()
        {
            var result = autoMocker.ClassUnderTest.Create(currentUser) as ViewResult;

            Assert.AreEqual(typeof(CreatePlayedGameViewModel), result.Model.GetType());
        }
    }
}
