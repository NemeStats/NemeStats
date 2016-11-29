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

using System.Collections.Generic;
using System.Linq;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Models;
using BusinessLogic.Models.Nemeses;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.NemesesTests.NemesisRecalculatorTests
{
    [TestFixture]
    public class RecalculateNemesisTests
    {
        private IPlayerRepository _playerRepositoryMock;
        private IDataContext _dataContextMock;
        private NemesisRecalculator _nemesisRecalculator;
        private ApplicationUser _currentUser;

        private int _playerId = 1;
        private Player _minionPlayer;
        private int _existingNemesisId = 15153;
        private int _newNemesisId = 9999;
        private Nemesis _savedNemesis;

        [SetUp]
        public void SetUp()
        {
            _playerRepositoryMock = MockRepository.GenerateMock<IPlayerRepository>();
            _dataContextMock = MockRepository.GenerateMock<IDataContext>();
            _nemesisRecalculator = new NemesisRecalculator(_dataContextMock, _playerRepositoryMock);

            _currentUser = new ApplicationUser();
            _minionPlayer = new Player()
            {
                NemesisId = _existingNemesisId
            };
            _dataContextMock.Expect(mock => mock.FindById<Player>(_playerId))
                .Return(_minionPlayer);
            _savedNemesis = new Nemesis() { Id = _newNemesisId };
            _dataContextMock.Expect(mock => mock.Save<Nemesis>(Arg<Nemesis>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Return(_savedNemesis);
        }

        [Test]
        public void ItReturnsANullNemesisIfTheNewNemesisIsNull()
        {
            _playerRepositoryMock.Expect(mock => mock.GetNemesisData(_playerId))
                                        .Return(new NullNemesisData());

            Nemesis nullNemesis = _nemesisRecalculator.RecalculateNemesis(_playerId, _currentUser);

            Assert.True(nullNemesis is NullNemesis);
        }

        [Test]
        public void ItClearsTheNemesisIfTheNewNemesisIsNullAndOneAlreadyExisted()
        {
            _playerRepositoryMock.Expect(mock => mock.GetNemesisData(_playerId))
                                        .Return(new NullNemesisData());

            _nemesisRecalculator.RecalculateNemesis(_playerId, _currentUser);

            _dataContextMock.AssertWasCalled(mock => mock.Save<Player>(
                Arg<Player>.Matches(player => player.NemesisId == null), 
                Arg<ApplicationUser>.Is.Equal(_currentUser)));
        }

        [Test]
        public void ItSetsTheNewNemesisIfItChanged()
        {
            NemesisData nemesisData = new NemesisData() { NemesisPlayerId = -1 };
            _playerRepositoryMock.Expect(mock => mock.GetNemesisData(_playerId))
                            .Return(nemesisData);

            _dataContextMock.Expect(mock => mock.GetQueryable<Nemesis>())
                .Return(new List<Nemesis>().AsQueryable());

            _nemesisRecalculator.RecalculateNemesis(_playerId, _currentUser);

            _dataContextMock.AssertWasCalled(mock => mock.Save<Nemesis>(
                Arg<Nemesis>.Matches(savedNemesis => savedNemesis.MinionPlayerId == _playerId
                                        && savedNemesis.NemesisPlayerId == nemesisData.NemesisPlayerId
                                        && savedNemesis.NumberOfGamesLost == nemesisData.NumberOfGamesLost
                                        && savedNemesis.LossPercentage == nemesisData.LossPercentage),
                Arg<ApplicationUser>.Is.Same(_currentUser)));
            _dataContextMock.AssertWasCalled(mock => mock.Save<Player>(
                Arg<Player>.Matches(player => player.NemesisId == _newNemesisId), Arg<ApplicationUser>.Is.Same(_currentUser)));
        }

        [Test]
        public void ItSetsThePreviousNemesisIfTheCurrentOneChanges()
        {
            NemesisData nemesisData = new NemesisData() { NemesisPlayerId = -1 };
            _playerRepositoryMock.Expect(mock => mock.GetNemesisData(_playerId))
                            .Return(nemesisData);

            _dataContextMock.Expect(mock => mock.GetQueryable<Nemesis>())
                .Return(new List<Nemesis>().AsQueryable());

            _nemesisRecalculator.RecalculateNemesis(_playerId, _currentUser);

            _dataContextMock.AssertWasCalled(mock => mock.Save<Player>(
                Arg<Player>.Matches(player => player.PreviousNemesisId == _existingNemesisId), Arg<ApplicationUser>.Is.Same(_currentUser)));
        }

        [Test]
        public void ItSetsThePreviousNemesisIfTheCurrentOneIsCleared()
        {
            _playerRepositoryMock.Expect(mock => mock.GetNemesisData(_playerId))
                            .Return(new NullNemesisData());

            _nemesisRecalculator.RecalculateNemesis(_playerId, _currentUser);

            _dataContextMock.AssertWasCalled(mock => mock.Save<Player>(
                Arg<Player>.Matches(player => player.PreviousNemesisId == _existingNemesisId),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItDoesntBotherSavingTheNemesisIfNothingHasChanged()
        {
            int nemesisPlayerId = 1;
            int gamesLost = 1;
            int lossPercentage = 1;
            NemesisData nemesisData = new NemesisData() 
            { 
                NemesisPlayerId = nemesisPlayerId,
                NumberOfGamesLost = gamesLost,
                LossPercentage = lossPercentage
            };
            _playerRepositoryMock.Expect(mock => mock.GetNemesisData(_playerId))
                            .Return(nemesisData);

            List<Nemesis> nemesisList = new List<Nemesis>();
            nemesisList.Add(new Nemesis() 
                                { 
                                    Id = _existingNemesisId,
                                    NemesisPlayerId = nemesisPlayerId,
                                    MinionPlayerId = _playerId,
                                    NumberOfGamesLost = gamesLost,
                                    LossPercentage = lossPercentage
                                });
            _dataContextMock.Expect(mock => mock.GetQueryable<Nemesis>())
                .Return(nemesisList.AsQueryable());

            _nemesisRecalculator.RecalculateNemesis(_playerId, _currentUser);

            _dataContextMock.AssertWasNotCalled(mock => mock.Save<Nemesis>(
                Arg<Nemesis>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItUpdatesTheExistingNemesisIfOnlyTheDataHasChanged()
        {
            int nemesisPlayerId = 1;
            int gamesLost = 1;
            int lossPercentage = 1;
            NemesisData nemesisData = new NemesisData()
            {
                NemesisPlayerId = nemesisPlayerId,
                NumberOfGamesLost = gamesLost,
                LossPercentage = lossPercentage
            };
            _playerRepositoryMock.Expect(mock => mock.GetNemesisData(_playerId))
                            .Return(nemesisData);

            List<Nemesis> nemesisList = new List<Nemesis>();
            Nemesis existingNemesis = new Nemesis()
            {
                Id = _existingNemesisId,
                NemesisPlayerId = nemesisPlayerId,
                MinionPlayerId = _playerId,
                //add 1 so the data is different
                NumberOfGamesLost = gamesLost + 1,
                LossPercentage = lossPercentage
            };
            nemesisList.Add(existingNemesis);
            _dataContextMock.Expect(mock => mock.GetQueryable<Nemesis>())
                .Return(nemesisList.AsQueryable());

            _nemesisRecalculator.RecalculateNemesis(_playerId, _currentUser);

            _dataContextMock.AssertWasCalled(mock => mock.Save<Nemesis>(
                Arg<Nemesis>.Matches(nem => nem.Id == _existingNemesisId
                                        && nem.MinionPlayerId == _playerId
                                        && nem.NemesisPlayerId == nemesisPlayerId
                                        && nem.NumberOfGamesLost == nemesisData.NumberOfGamesLost
                                        && nem.LossPercentage == nemesisData.LossPercentage),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItReturnsTheExistingNemesisIfNothingChanged()
        {
            NemesisData nemesisData = new NemesisData();
            _playerRepositoryMock.Expect(mock => mock.GetNemesisData(_playerId))
                            .Return(nemesisData);

            List<Nemesis> nemesisList = new List<Nemesis>();
            Nemesis existingNemesis = new Nemesis()
            {
                Id = _existingNemesisId,
                MinionPlayerId = _playerId
            };
            nemesisList.Add(existingNemesis);
            _dataContextMock.Expect(mock => mock.GetQueryable<Nemesis>())
                .Return(nemesisList.AsQueryable());

            Nemesis actualNemesis = _nemesisRecalculator.RecalculateNemesis(_playerId, _currentUser);

            Assert.AreSame(existingNemesis, actualNemesis);
        }

        [Test]
        public void ItReturnsTheUpdatedNemesisIfItWasUpdated()
        {
            int expectedLossPercentage = 15;
            NemesisData nemesisData = new NemesisData() { LossPercentage = expectedLossPercentage };
            _playerRepositoryMock.Expect(mock => mock.GetNemesisData(_playerId))
                            .Return(nemesisData);

            List<Nemesis> nemesisList = new List<Nemesis>();
            Nemesis existingNemesis = new Nemesis()
            {
                Id = _existingNemesisId,
                MinionPlayerId = _playerId
                
            };
            nemesisList.Add(existingNemesis);
            _dataContextMock.Expect(mock => mock.GetQueryable<Nemesis>())
                .Return(nemesisList.AsQueryable());

            Nemesis actualNemesis = _nemesisRecalculator.RecalculateNemesis(_playerId, _currentUser);

            Assert.AreSame(_savedNemesis, actualNemesis);
        }

        [Test]
        public void ItReturnsTheNewNemesisIfItWasChanged()
        {
            //change the nemesis
            NemesisData nemesisData = new NemesisData() { NemesisPlayerId = 19383 };
            _playerRepositoryMock.Expect(mock => mock.GetNemesisData(_playerId))
                            .Return(nemesisData);

            List<Nemesis> nemesisList = new List<Nemesis>();
            Nemesis existingNemesis = new Nemesis()
            {
                Id = _existingNemesisId,
                MinionPlayerId = _playerId

            };
            nemesisList.Add(existingNemesis);
            _dataContextMock.Expect(mock => mock.GetQueryable<Nemesis>())
                .Return(nemesisList.AsQueryable());

            Nemesis actualNemesis = _nemesisRecalculator.RecalculateNemesis(_playerId, _currentUser);

            Assert.AreSame(_savedNemesis, actualNemesis);
        }
    }
}
