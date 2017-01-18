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

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayedGamesTests.PlayedGameDeleterTests
{
    [TestFixture]
    public class DeletePlayedGameTests
    {
        private IDataContext _dataContextMock;
        private INemesisRecalculator _nemesisRecalculatorMock;
        private IChampionRecalculator _championRecalculatorMock;
        private PlayedGameDeleter _playedGameDeleter;
        private ApplicationUser _currentUser;
        private List<PlayerGameResult> _playerGameResults;
        private int _playedGameId = 1;
        private int _playerInGame1Id = 1;
        private int _playerInGame2Id = 2;
        private int _playerNotInGame = 9999;
        private int _gameDefinitionId = 40;
        private PlayedGame _playedGame;

        [SetUp]
        public void SetUp()
        {
            _dataContextMock = MockRepository.GenerateMock<IDataContext>();
            _nemesisRecalculatorMock = MockRepository.GenerateMock<INemesisRecalculator>();
            _championRecalculatorMock = MockRepository.GenerateMock<IChampionRecalculator>();
            _playedGameDeleter = new PlayedGameDeleter(_dataContextMock, _nemesisRecalculatorMock, _championRecalculatorMock);

            _currentUser = new ApplicationUser();

            _playedGame = new PlayedGame()
            {
                GameDefinitionId = _gameDefinitionId
            };

            _playerGameResults = new List<PlayerGameResult>()
            {
                new PlayerGameResult(){ PlayerId = _playerInGame1Id, PlayedGameId = _playedGameId, PlayedGame = _playedGame },
                new PlayerGameResult(){ PlayerId = _playerInGame2Id, PlayedGameId = _playedGameId, PlayedGame = _playedGame },
                new PlayerGameResult(){ PlayerId = 3, PlayedGameId = _playedGameId + 9999, PlayedGame = _playedGame }
            };

            _dataContextMock.Expect(mock => mock.GetQueryable<PlayerGameResult>())
                .Return(_playerGameResults.AsQueryable());
        }

        [Test]
        public void ItDeletesTheSpecifiedPlayedGame()
        {
            _playedGameDeleter.DeletePlayedGame(_playedGameId, _currentUser);

            _dataContextMock.AssertWasCalled(mock => mock.DeleteById<PlayedGame>(_playedGameId, _currentUser));
        }

        [Test]
        public void ItRecalculatesTheNemesesOfAllPlayersInTheDeletedGame()
        {
            int playedGameIdId = 1;
            _playedGameDeleter.DeletePlayedGame(playedGameIdId, _currentUser);

            _nemesisRecalculatorMock.AssertWasCalled(mock => mock.RecalculateNemesis(_playerInGame1Id, _currentUser, _dataContextMock));
            _nemesisRecalculatorMock.AssertWasCalled(mock => mock.RecalculateNemesis(_playerInGame2Id, _currentUser, _dataContextMock));
            _nemesisRecalculatorMock.AssertWasNotCalled(mock => mock.RecalculateNemesis(_playerNotInGame, _currentUser, _dataContextMock));
        }

        [Test]
        public void ItRecalculatesTheChampionOfTheDeletedGame()
        {
            int playedGameIdId = 1;
            _playedGameDeleter.DeletePlayedGame(playedGameIdId, _currentUser);

            _championRecalculatorMock.AssertWasCalled(mock => mock.RecalculateChampion(_gameDefinitionId, _currentUser, _dataContextMock));
        }
    }
}
