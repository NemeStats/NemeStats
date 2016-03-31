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
using BusinessLogic.Logic.Champions;
using BusinessLogic.Models;
using BusinessLogic.Models.Champions;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.AutoMocking;
using Is = NUnit.Framework.Is;

namespace BusinessLogic.Tests.UnitTests.LogicTests.ChampionsTests.ChampionRecalculatorTests
{
    [TestFixture]
    public class RecalculateChampionTests
    {
        private RhinoAutoMocker<ChampionRecalculator> _autoMocker; 
        private ApplicationUser _applicationUser;

        private readonly int _gameDefinitionId = 1;
        private GameDefinition _gameDefinition;
        private readonly int _previousChampionId = 75;
        private readonly int _newChampionId = 100;
        private readonly int _playerChampionId = 99;
        private Champion _savedChampion;

        [SetUp]
        public void SetUp()
        {
            _autoMocker = new RhinoAutoMocker<ChampionRecalculator>();
            _applicationUser = new ApplicationUser();

            _gameDefinition = new GameDefinition
            {
                ChampionId = _previousChampionId
            };
            _autoMocker.Get<IDataContext>().Expect(mock => mock.FindById<GameDefinition>(_gameDefinitionId))
                .Return(_gameDefinition);
            _savedChampion = new Champion { Id = _newChampionId };
            _autoMocker.Get<IDataContext>().Expect(mock => mock.Save(Arg<Champion>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Return(_savedChampion);

        }

        [Test]
        public void ItReturnsANullChampionIfTheChampionWasClearedOutWithNoReplacement()
        {
            _autoMocker.Get<IChampionRepository>().Expect(mock => mock.GetChampionData(_gameDefinitionId))
                .Return(new NullChampionData());

            Champion nullChampion = _autoMocker.ClassUnderTest.RecalculateChampion(_gameDefinitionId, _applicationUser);

            Assert.That(nullChampion, Is.InstanceOf<NullChampion>());
        }

        [Test]
        public void ItRemovesTheChampionIfAllowedAndTheNewChampionIsNullAndOneAlreadyExisted()
        {
            _autoMocker.Get<IChampionRepository>().Expect(mock => mock.GetChampionData(_gameDefinitionId))
                .Return(new NullChampionData());

            _autoMocker.ClassUnderTest.RecalculateChampion(_gameDefinitionId, _applicationUser, true);

            _autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                Arg<GameDefinition>.Matches(gameDefinition => gameDefinition.ChampionId == null), 
                Arg<ApplicationUser>.Is.Equal(_applicationUser)));
        }

        [Test]
        public void ItDoesNotRemoveTheExistingChampionIfNotAllowedAndTheNewChampionIsNullAndOneAlreadyExisted()
        {
            _autoMocker.Get<IChampionRepository>().Expect(mock => mock.GetChampionData(_gameDefinitionId))
                .Return(new NullChampionData());

            _autoMocker.ClassUnderTest.RecalculateChampion(_gameDefinitionId, _applicationUser, false);

            _autoMocker.Get<IDataContext>().AssertWasNotCalled(mock => mock.Save(
                Arg<GameDefinition>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItSetsTheNewChampionIfItChanged()
        {
            ChampionData championData = new ChampionData { PlayerId = -1 };
            _autoMocker.Get<IChampionRepository>().Expect(mock => mock.GetChampionData(_gameDefinitionId))
                .Return(championData);

            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Champion>())
                .Return(new List<Champion>().AsQueryable());

            _autoMocker.ClassUnderTest.RecalculateChampion(_gameDefinitionId, _applicationUser);

            _autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                Arg<Champion>.Matches(champion => champion.GameDefinitionId == _gameDefinitionId
                && champion.PlayerId == championData.PlayerId
                && champion.WinPercentage == championData.WinPercentage), 
                Arg<ApplicationUser>.Is.Same(_applicationUser)));
            _autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                Arg<GameDefinition>.Matches(definition => definition.ChampionId == _newChampionId), 
                Arg<ApplicationUser>.Is.Same(_applicationUser)));
        }

        [Test]
        public void ItSetsThePreviousChampionIfTheCurrentOneChangesButItIsTheSamePlayer()
        {
            ChampionData championData = new ChampionData {PlayerId = -1};
            _autoMocker.Get<IChampionRepository>().Expect(mock => mock.GetChampionData(_gameDefinitionId))
                .Return(championData);

            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Champion>())
                .Return(new List<Champion>().AsQueryable());

            _autoMocker.ClassUnderTest.RecalculateChampion(_gameDefinitionId, _applicationUser);

            _autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                Arg<GameDefinition>.Matches(definition => definition.PreviousChampionId == _previousChampionId),
                Arg<ApplicationUser>.Is.Same(_applicationUser)));
        }

        [Test]
        public void ItSetsThePreviousChampioinIfTheCurrentOneIsCleared()
        {
            _autoMocker.Get<IChampionRepository>().Expect(mock => mock.GetChampionData(_gameDefinitionId))
                .Return(new NullChampionData());

            _autoMocker.ClassUnderTest.RecalculateChampion(_gameDefinitionId, _applicationUser);

            _autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                Arg<GameDefinition>.Matches(definition => definition.PreviousChampionId == _previousChampionId),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItDoesntSaveTheChampionIfNothingChanged()
        {
            int championPlayerId = 1;
            int winPercentage = 1;
            ChampionData championData = new ChampionData
            {
                GameDefinitionId = _gameDefinitionId,
                PlayerId = championPlayerId,
                WinPercentage = winPercentage
            };

            _autoMocker.Get<IChampionRepository>().Expect(mock => mock.GetChampionData(_gameDefinitionId))
                .Return(championData);

            List<Champion> championList = new List<Champion>();
            championList.Add(new Champion
            {
                Id = _previousChampionId,
                PlayerId = championPlayerId,
                GameDefinitionId = _gameDefinitionId,
                WinPercentage = winPercentage,
                GameDefinition = new GameDefinition { ChampionId = _previousChampionId }
            });
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Champion>())
                .Return(championList.AsQueryable());

            _autoMocker.ClassUnderTest.RecalculateChampion(_gameDefinitionId, _applicationUser);

            _autoMocker.Get<IDataContext>().AssertWasNotCalled(mock => mock.Save(
                Arg<Champion>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItUpdatesTheExistingChampionIfOnlyTheDataHasChanged()
        {
            int championPlayerId = 1;
            int winPercentage = 1;
            ChampionData championData = new ChampionData
            {
                WinPercentage = winPercentage,
                PlayerId = championPlayerId,
                GameDefinitionId = _gameDefinitionId
            };
            _autoMocker.Get<IChampionRepository>().Expect(mock => mock.GetChampionData(_gameDefinitionId))
                .Return(championData);

            List<Champion> championList = new List<Champion>();
            Champion existingChampion = new Champion
            {
                Id = _previousChampionId,
                PlayerId = championPlayerId,
                WinPercentage = winPercentage + 1,
                GameDefinitionId = _gameDefinitionId,
                GameDefinition = new GameDefinition()
            };
            championList.Add(existingChampion);

            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Champion>())
                .Return(championList.AsQueryable());

            _autoMocker.ClassUnderTest.RecalculateChampion(_gameDefinitionId, _applicationUser);

            _autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                Arg<Champion>.Matches(champion => champion.GameDefinitionId == _gameDefinitionId
                                        && champion.PlayerId == championPlayerId
                                        && champion.WinPercentage == winPercentage),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItReturnsTheExistingChampionIfNothingChanged()
        {
            ChampionData championData = new ChampionData
            {
                GameDefinitionId = _gameDefinitionId,
                PlayerId = _playerChampionId
            };
            _autoMocker.Get<IChampionRepository>().Expect(mock => mock.GetChampionData(_gameDefinitionId))
                .Return(championData);

            List<Champion> championList = new List<Champion>();
            Champion existingChampion = new Champion
            {
                Id = _previousChampionId,
                PlayerId = _playerChampionId,
                GameDefinitionId = _gameDefinitionId,
                GameDefinition = new GameDefinition { ChampionId = _previousChampionId }
            };
            championList.Add(existingChampion);
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Champion>())
                .Return(championList.AsQueryable());

            Champion actualChampion = _autoMocker.ClassUnderTest.RecalculateChampion(_gameDefinitionId, _applicationUser);

            Assert.That(actualChampion, Is.SameAs(existingChampion));
        }

        [Test]
        public void ItReturnsTheUpdatedChampionIfItWasUpdated()
        {
            int expectedWinPercentage = 85;
            ChampionData championData = new ChampionData {WinPercentage = expectedWinPercentage};
            _autoMocker.Get<IChampionRepository>().Expect(mock => mock.GetChampionData(_gameDefinitionId))
                .Return(championData);

            List<Champion> championList = new List<Champion>();
            Champion existingChampion = new Champion
            {
                Id = _previousChampionId,
                PlayerId = _playerChampionId,
                GameDefinitionId = _gameDefinitionId,
                GameDefinition = new GameDefinition()
            };
            championList.Add(existingChampion);
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Champion>())
                .Return(championList.AsQueryable());

            Champion actualChampion = _autoMocker.ClassUnderTest.RecalculateChampion(_gameDefinitionId, _applicationUser);

            Assert.That(actualChampion, Is.SameAs(_savedChampion));
        }

        [Test]
        public void ItReturnsTheNewChampionIfItWasChanged()
        {
            ChampionData championData = new ChampionData {PlayerId = 10000};
            _autoMocker.Get<IChampionRepository>().Expect(mock => mock.GetChampionData(_gameDefinitionId))
                .Return(championData);

            List<Champion> championList = new List<Champion>();
            Champion existingChampion = new Champion
            {
                Id = _previousChampionId,
                PlayerId = _playerChampionId
            };
            championList.Add(existingChampion);
            _autoMocker.Get<IDataContext>().Expect(mock => mock.GetQueryable<Champion>())
                .Return(championList.AsQueryable());

            Champion actualChampion = _autoMocker.ClassUnderTest.RecalculateChampion(_gameDefinitionId, _applicationUser);

            Assert.That(actualChampion, Is.SameAs(_savedChampion));
        }
    }
}
