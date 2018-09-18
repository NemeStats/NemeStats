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
using BusinessLogic.EventTracking;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BusinessLogic.Logic.Champions;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayersTests.PlayerSaverTests
{
    [TestFixture]
    public class SaveTests : PlayerSaverTestBase
    { 
        [Test]
        public void ItThrowsAnArgumentNullExceptionIfThePlayerIsNull()
        {
            var expectedException = new ArgumentNullException("player");

            Exception exception = Assert.Throws<ArgumentNullException>(() => _autoMocker.ClassUnderTest.Save((Player)null, _currentUser));

            Assert.AreEqual(expectedException.Message, exception.Message);
        }

        [Test]
        public void ItThrowsAnArgumentNullExceptionIfThePlayerNameIsWhitespace()
        {
            var player = new Player
            {
                Name = "    "
            };
            var expectedException = new ArgumentNullException("playerName");

            Exception exception = Assert.Throws<ArgumentNullException>(() => _autoMocker.ClassUnderTest.Save(player, _currentUser));

            Assert.AreEqual(expectedException.Message, exception.Message);
        }

        [Test]
        public void ItSetsThePlayerName()
        {
            var player = new Player
            {
                Name = "player name"
            };

            _autoMocker.ClassUnderTest.Save(player, _currentUser);

            _autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                Arg<Player>.Matches(savedPlayer => savedPlayer.Name == player.Name), 
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void TheNewPlayerIsActiveWhenCreated()
        {
            _autoMocker.ClassUnderTest.Save(new Player
            { Name = "player name" }, _currentUser);

            _autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                Arg<Player>.Matches(player => player.Active),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test, Ignore("This test is flaky due to fire-and-forget task")]
        public void ItRecordsAPlayerCreatedEventIfThePlayerIsNew()
        {
            var player = MockRepository.GeneratePartialMock<Player>();
            player.Name = "player name";
            player.Expect(mock => mock.AlreadyInDatabase())
                .Return(false);

            _autoMocker.ClassUnderTest.Save(player, _currentUser);

            try
            {
                _autoMocker.Get<INemeStatsEventTracker>().AssertWasCalled(mock => mock.TrackPlayerCreation(_currentUser));
            }catch(Exception)
            {
                //since this happens in a task there can be a race condition where the test runs before this method is called. Hopefully this
                // solves the problem
                Thread.Sleep(200);
                _autoMocker.Get<INemeStatsEventTracker>().AssertWasCalled(mock => mock.TrackPlayerCreation(_currentUser));

            }
        }

        [Test]
        public void ItDoesNotRecordAPlayerCreatedEventIfThePlayerIsNotNew()
        {
            var player = MockRepository.GenerateMock<Player>();
            player.Name = "player name";
            player.Expect(mock => mock.AlreadyInDatabase())
                .Return(true);

            _autoMocker.ClassUnderTest.Save(player, _currentUser);

            _autoMocker.Get<INemeStatsEventTracker>().AssertWasNotCalled(mock => mock.TrackPlayerCreation(_currentUser));
        }

        [Test]
        public void ItRecalculatesTheNemesisOfTheCurrentPlayersMinionsIfThePlayerIsGoingInactive()
        {
            var player = MockRepository.GeneratePartialMock<Player>();
            player.Name = "player name";
            player.Active = false;
            player.Id = 151516;
            player.Expect(mock => mock.AlreadyInDatabase())
                .Return(true);

            const int EXPECTED_PLAYER_ID1 = 1;
            const int EXPECTED_PLAYER_ID2 = 2;

            var activeMinion1 = new Player
            {
                Id = EXPECTED_PLAYER_ID1
            };
            var activeNemesis = new Nemesis
            { 
                NemesisPlayerId = player.Id,
            };
            activeMinion1.Nemesis = activeNemesis;

            var activeMinion2 = new Player
            {
                Id = EXPECTED_PLAYER_ID2
            };
            var secondActiveNemesis = new Nemesis
            { 
                NemesisPlayerId = player.Id,
            };
            activeMinion2.Nemesis = secondActiveNemesis;

            var inactiveMinion = new Player
            {
                Id = -1,
                Active = false
            };
            var inactiveNemesis = new Nemesis
            { 
                NemesisPlayerId = player.Id
            };
            inactiveMinion.Nemesis = inactiveNemesis;

            var minionPlayers = new List<Player>
            {
                activeMinion1,
                activeMinion2,
                inactiveMinion
            }.AsQueryable();

            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Player>())
                .Repeat.Any()
                .Return(minionPlayers);

            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>())
                .Return(new List<GameDefinition>().AsQueryable());

            _autoMocker.ClassUnderTest.Save(player, _currentUser);
            
            _autoMocker.Get<INemesisRecalculator>().AssertWasCalled(mock => mock.RecalculateNemesis(activeMinion1.Id, _currentUser, _autoMocker.Get<IDataContext>()));
            _autoMocker.Get<INemesisRecalculator>().AssertWasCalled(mock => mock.RecalculateNemesis(activeMinion2.Id, _currentUser, _autoMocker.Get<IDataContext>()));
            _autoMocker.Get<INemesisRecalculator>().AssertWasNotCalled(mock => mock.RecalculateNemesis(inactiveMinion.Id, _currentUser, _autoMocker.Get<IDataContext>()));
        }

        [Test]
        public void ItRecalculatesTheChampionsOfThePlayersChampionedGamesIfThePlayerIsGoingInactive()
        {
            var player = MockRepository.GeneratePartialMock<Player>();
            player.Active = false;
            player.Name = "some name";
            player.Id = 151516;
            player.Expect(mock => mock.AlreadyInDatabase())
                .Return(true);

            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Player>())
                .Repeat.Any()
                .Return(new List<Player>().AsQueryable());

            var gameDefinitionId1 = 10;
            var gameDefinitionId2 = 11;
            var gameDefinitionIdNotForPlayer = 99;

            var gameDefinitions = new List<GameDefinition>
            {
                new GameDefinition
                {
                    Id = gameDefinitionId1,
                    Champion = new Champion
                    {
                        PlayerId = player.Id
                    }
                },
                new GameDefinition
                {
                    Id = gameDefinitionId2,
                    Champion = new Champion
                    {
                        PlayerId = player.Id
                    }
                },
                //--this one should not get picked up
                new GameDefinition
                {
                    Id = gameDefinitionIdNotForPlayer,
                    Champion = new Champion
                    {
                        PlayerId = -1
                    }
                }
            }.AsQueryable();
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<GameDefinition>())
                .Return(gameDefinitions);

            _autoMocker.ClassUnderTest.Save(player, _currentUser);

            _autoMocker.Get<IChampionRecalculator>().AssertWasCalled(mock => mock.RecalculateChampion(gameDefinitionId1, _currentUser, _autoMocker.Get<IDataContext>()));
            _autoMocker.Get<IChampionRecalculator>().AssertWasCalled(mock => mock.RecalculateChampion(gameDefinitionId2, _currentUser, _autoMocker.Get<IDataContext>()));

            _autoMocker.Get<IChampionRecalculator>().AssertWasNotCalled(mock => mock.RecalculateChampion(gameDefinitionIdNotForPlayer, _currentUser, _autoMocker.Get<IDataContext>()));
        }

        [Test]
        public void ItDoesNotRecalculateTheNemesisOfTheCurrentPlayersMinionsIfThePlayerIsStillActive()
        {
            var player = MockRepository.GeneratePartialMock<Player>();
            player.Name = "player name";
            player.Active = true;
            player.Id = 151516;

            player.Expect(mock => mock.AlreadyInDatabase())
                .Return(true);

            const int CURRENT_PLAYER_MINION_ID1 = 10;

            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Player>())
    .           Return(new List<Player>().AsQueryable());

            _autoMocker.ClassUnderTest.Save(player, _currentUser);

            _autoMocker.Get<INemesisRecalculator>().AssertWasNotCalled(mock => mock.RecalculateNemesis(CURRENT_PLAYER_MINION_ID1, _currentUser, _autoMocker.Get<IDataContext>()));
        }

        [Test]
        public void ItThrowsAPlayerAlreadyExistsExceptionIfAttemptingToSaveAPlayerWithANameThatAlreadyExists()
        {
            var player = new Player
            {
                Name = _playerThatAlreadyExists.Name,
                GamingGroupId = _currentUser.CurrentGamingGroupId.Value
            };
            var exception = Assert.Throws<PlayerAlreadyExistsException>(
                () => _autoMocker.ClassUnderTest.Save(player, _currentUser));

            Assert.AreEqual(_idOfPlayerThatAlreadyExists, exception.ExistingPlayerId);
        }
    }
}
