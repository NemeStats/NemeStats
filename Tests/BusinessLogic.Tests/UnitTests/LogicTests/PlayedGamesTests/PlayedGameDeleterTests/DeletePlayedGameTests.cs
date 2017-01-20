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
using BusinessLogic.DataAccess;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models.User;
using BusinessLogic.Models;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Logic.Champions;
using BusinessLogic.Logic.MVP;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayedGamesTests.PlayedGameDeleterTests
{
    [TestFixture]
    public class DeletePlayedGameTests
    {
        private IDataContext dataContextMock;
        private INemesisRecalculator nemesisRecalculatorMock;
        private IChampionRecalculator championRecalculatorMock;
        private IMVPRecalculator mvpRecalculatorMock;
        private PlayedGameDeleter playedGameDeleter;
        private ApplicationUser currentUser;
        private List<PlayerGameResult> playerGameResults;
        private int playedGameId = 1;
        private int playerInGame1Id = 1;
        private int playerInGame2Id = 2;
        private int playerNotInGame = 9999;
        private int gameDefinitionId = 40;
        private PlayedGame playedGame;

        [SetUp]
        public void SetUp()
        {
            dataContextMock = MockRepository.GenerateMock<IDataContext>();
            nemesisRecalculatorMock = MockRepository.GenerateMock<INemesisRecalculator>();
            championRecalculatorMock = MockRepository.GenerateMock<IChampionRecalculator>();
            mvpRecalculatorMock = MockRepository.GenerateMock<IMVPRecalculator>();
            playedGameDeleter = new PlayedGameDeleter(dataContextMock, nemesisRecalculatorMock, championRecalculatorMock, mvpRecalculatorMock);

            currentUser = new ApplicationUser();

            playedGame = new PlayedGame()
            {
                GameDefinitionId = gameDefinitionId
            };

            playerGameResults = new List<PlayerGameResult>()
            {
                new PlayerGameResult(){ Id = 1,  PlayerId = playerInGame1Id, PlayedGameId = playedGameId, PlayedGame = playedGame },
                new PlayerGameResult(){ Id = 2, PlayerId = playerInGame2Id, PlayedGameId = playedGameId, PlayedGame = playedGame },
                new PlayerGameResult(){ Id = 3, PlayerId = 3, PlayedGameId = playedGameId + 9999, PlayedGame = playedGame }
            };

            dataContextMock.Expect(mock => mock.GetQueryable<PlayerGameResult>()).Return(playerGameResults.AsQueryable());
            dataContextMock.Expect(mock => mock.GetQueryable<Models.MVP>()).Return(new List<MVP>().AsQueryable());

        }

        [Test]
        public void ItDeletesTheSpecifiedPlayedGame()
        {
            playedGameDeleter.DeletePlayedGame(playedGameId, currentUser);

            dataContextMock.AssertWasCalled(mock => mock.DeleteById<PlayedGame>(playedGameId, currentUser));
        }

        [Test]
        public void ItRecalculatesTheNemesesOfAllPlayersInTheDeletedGame()
        {
            int playedGameIdId = 1;
            playedGameDeleter.DeletePlayedGame(playedGameIdId, currentUser);

            nemesisRecalculatorMock.AssertWasCalled(mock => mock.RecalculateNemesis(playerInGame1Id, currentUser));
            nemesisRecalculatorMock.AssertWasCalled(mock => mock.RecalculateNemesis(playerInGame2Id, currentUser));
            nemesisRecalculatorMock.AssertWasNotCalled(mock => mock.RecalculateNemesis(playerNotInGame, currentUser));
        }

        [Test]
        public void ItRecalculatesTheChampionOfTheDeletedGame()
        {
            int playedGameIdId = 1;
            playedGameDeleter.DeletePlayedGame(playedGameIdId, currentUser);

            championRecalculatorMock.AssertWasCalled(mock => mock.RecalculateChampion(gameDefinitionId, currentUser));
        }

        [Test]
        public void ItRecalculatesTheMVPfTheDeletedGame()
        {
            int playedGameIdId = 1;
            playedGameDeleter.DeletePlayedGame(playedGameIdId, currentUser);

            mvpRecalculatorMock.AssertWasCalled(mock => mock.RecalculateMVP(gameDefinitionId, currentUser));
        }
    }
}
