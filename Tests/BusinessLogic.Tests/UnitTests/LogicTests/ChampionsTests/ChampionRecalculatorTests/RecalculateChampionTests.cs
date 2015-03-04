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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.Champions;
using BusinessLogic.Models;
using BusinessLogic.Models.Champions;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using Is = NUnit.Framework.Is;
using List = NUnit.Framework.List;

namespace BusinessLogic.Tests.UnitTests.LogicTests.ChampionsTests.ChampionRecalculatorTests
{
    [TestFixture]
    public class RecalculateChampionTests
    {
        private IChampionRepository championRepositoryMock;
        private IDataContext dataContextMock;
        private ChampionRecalculator championRecalculator;
        private ApplicationUser applicationUser;

        private int gameDefinitionId = 1;
        private GameDefinition gameDefinition;
        private int previousChampionId = 75;
        private int newChampionId = 100;
        private int playerChampionId = 99;
        private Champion savedChampion;

        [SetUp]
        public void SetUp()
        {
            championRepositoryMock = MockRepository.GenerateMock<IChampionRepository>();
            dataContextMock = MockRepository.GenerateMock<IDataContext>();
            championRecalculator = new ChampionRecalculator(dataContextMock, championRepositoryMock);
            applicationUser = new ApplicationUser();

            gameDefinition = new GameDefinition
            {
                ChampionId = previousChampionId
            };
            dataContextMock.Expect(mock => mock.FindById<GameDefinition>(gameDefinitionId))
                .Return(gameDefinition);
            savedChampion = new Champion { Id = newChampionId };
            dataContextMock.Expect(mock => mock.Save(Arg<Champion>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Return(savedChampion);

        }

        [Test]
        public void ItReturnsANullChampionIfTheNewChampionIsNull()
        {
            championRepositoryMock.Expect(mock => mock.GetChampionData(gameDefinitionId))
                .Return(new NullChampionData());

            Champion nullChampion = championRecalculator.RecalculateChampion(gameDefinitionId, applicationUser);

            Assert.That(nullChampion, Is.InstanceOf<NullChampion>());
        }

        [Test]
        public void ItRemovesTheChampionIfTheNewChampionIsNullAndOneAlreadyExisted()
        {
            championRepositoryMock.Expect(mock => mock.GetChampionData(gameDefinitionId))
                .Return(new NullChampionData());

            championRecalculator.RecalculateChampion(gameDefinitionId, applicationUser);

            dataContextMock.AssertWasCalled(mock => mock.Save(
                Arg<GameDefinition>.Matches(gameDefinition => gameDefinition.ChampionId == null), 
                Arg<ApplicationUser>.Is.Equal(applicationUser)));
        }

        [Test]
        public void ItSetsTheNewChampionIfItChanged()
        {
            ChampionData championData = new ChampionData { PlayerId = -1 };
            championRepositoryMock.Expect(mock => mock.GetChampionData(gameDefinitionId))
                .Return(championData);

            dataContextMock.Expect(mock => mock.GetQueryable<Champion>())
                .Return(new List<Champion>().AsQueryable());

            championRecalculator.RecalculateChampion(gameDefinitionId, applicationUser);

            dataContextMock.AssertWasCalled(mock => mock.Save(
                Arg<Champion>.Matches(champion => champion.GameDefinitionId == gameDefinitionId
                && champion.PlayerId == championData.PlayerId
                && champion.WinPercentage == championData.WinPercentage), 
                Arg<ApplicationUser>.Is.Same(applicationUser)));
            dataContextMock.AssertWasCalled(mock => mock.Save(
                Arg<GameDefinition>.Matches(definition => definition.ChampionId == newChampionId), 
                Arg<ApplicationUser>.Is.Same(applicationUser)));
        }

        [Test]
        public void ItSetsThePreviousChampionIfTheCurrentOneChangesButItIsTheSamePlayer()
        {
            ChampionData championData = new ChampionData {PlayerId = -1};
            championRepositoryMock.Expect(mock => mock.GetChampionData(gameDefinitionId))
                .Return(championData);

            dataContextMock.Expect(mock => mock.GetQueryable<Champion>())
                .Return(new List<Champion>().AsQueryable());

            championRecalculator.RecalculateChampion(gameDefinitionId, applicationUser);

            dataContextMock.AssertWasCalled(mock => mock.Save(
                Arg<GameDefinition>.Matches(definition => definition.PreviousChampionId == previousChampionId),
                Arg<ApplicationUser>.Is.Same(applicationUser)));
        }

        [Test]
        public void ItSetsThePreviousChampioinIfTheCurrentOneIsCleared()
        {
            championRepositoryMock.Expect(mock => mock.GetChampionData(gameDefinitionId))
                .Return(new NullChampionData());

            championRecalculator.RecalculateChampion(gameDefinitionId, applicationUser);

            dataContextMock.AssertWasCalled(mock => mock.Save(
                Arg<GameDefinition>.Matches(definition => definition.PreviousChampionId == previousChampionId),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItDoesntSaveTheChampionIfNothingChanged()
        {
            int championPlayerId = 1;
            int winPercentage = 1;
            ChampionData championData = new ChampionData
            {
                GameDefinitionId = gameDefinitionId,
                PlayerId = championPlayerId,
                WinPercentage = winPercentage
            };

            championRepositoryMock.Expect(mock => mock.GetChampionData(gameDefinitionId))
                .Return(championData);

            List<Champion> championList = new List<Champion>();
            championList.Add(new Champion
            {
                Id = previousChampionId,
                PlayerId = championPlayerId,
                GameDefinitionId = gameDefinitionId,
                WinPercentage = winPercentage,
                GameDefinition = new GameDefinition { ChampionId = previousChampionId }
            });
            dataContextMock.Expect(mock => mock.GetQueryable<Champion>())
                .Return(championList.AsQueryable());

            championRecalculator.RecalculateChampion(gameDefinitionId, applicationUser);

            dataContextMock.AssertWasNotCalled(mock => mock.Save(
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
                GameDefinitionId = gameDefinitionId
            };
            championRepositoryMock.Expect(mock => mock.GetChampionData(gameDefinitionId))
                .Return(championData);

            List<Champion> championList = new List<Champion>();
            Champion existingChampion = new Champion
            {
                Id = previousChampionId,
                PlayerId = championPlayerId,
                WinPercentage = winPercentage + 1,
                GameDefinitionId = gameDefinitionId,
                GameDefinition = new GameDefinition()
            };
            championList.Add(existingChampion);

            dataContextMock.Expect(mock => mock.GetQueryable<Champion>())
                .Return(championList.AsQueryable());

            championRecalculator.RecalculateChampion(gameDefinitionId, applicationUser);

            dataContextMock.AssertWasCalled(mock => mock.Save(
                Arg<Champion>.Matches(champion => champion.GameDefinitionId == gameDefinitionId
                                        && champion.PlayerId == championPlayerId
                                        && champion.WinPercentage == winPercentage),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItReturnsTheExistingChampionIfNothingChanged()
        {
            ChampionData championData = new ChampionData
            {
                GameDefinitionId = gameDefinitionId,
                PlayerId = playerChampionId
            };
            championRepositoryMock.Expect(mock => mock.GetChampionData(gameDefinitionId))
                .Return(championData);

            List<Champion> championList = new List<Champion>();
            Champion existingChampion = new Champion
            {
                Id = previousChampionId,
                PlayerId = playerChampionId,
                GameDefinitionId = gameDefinitionId,
                GameDefinition = new GameDefinition { ChampionId = previousChampionId }
            };
            championList.Add(existingChampion);
            dataContextMock.Expect(mock => mock.GetQueryable<Champion>())
                .Return(championList.AsQueryable());

            Champion actualChampion = championRecalculator.RecalculateChampion(gameDefinitionId, applicationUser);

            Assert.That(actualChampion, Is.SameAs(existingChampion));
        }

        [Test]
        public void ItReturnsTheUpdatedChampionIfItWasUpdated()
        {
            int expectedWinPercentage = 85;
            ChampionData championData = new ChampionData {WinPercentage = expectedWinPercentage};
            championRepositoryMock.Expect(mock => mock.GetChampionData(gameDefinitionId))
                .Return(championData);

            List<Champion> championList = new List<Champion>();
            Champion existingChampion = new Champion
            {
                Id = previousChampionId,
                PlayerId = playerChampionId,
                GameDefinitionId = gameDefinitionId,
                GameDefinition = new GameDefinition()
            };
            championList.Add(existingChampion);
            dataContextMock.Expect(mock => mock.GetQueryable<Champion>())
                .Return(championList.AsQueryable());

            Champion actualChampion = championRecalculator.RecalculateChampion(gameDefinitionId, applicationUser);

            Assert.That(actualChampion, Is.SameAs(savedChampion));
        }

        [Test]
        public void ItReturnsTheNewChampionIfItWasChanged()
        {
            ChampionData championData = new ChampionData {PlayerId = 10000};
            championRepositoryMock.Expect(mock => mock.GetChampionData(gameDefinitionId))
                .Return(championData);

            List<Champion> championList = new List<Champion>();
            Champion existingChampion = new Champion
            {
                Id = previousChampionId,
                PlayerId = playerChampionId
            };
            championList.Add(existingChampion);
            dataContextMock.Expect(mock => mock.GetQueryable<Champion>())
                .Return(championList.AsQueryable());

            Champion actualChampion = championRecalculator.RecalculateChampion(gameDefinitionId, applicationUser);

            Assert.That(actualChampion, Is.SameAs(savedChampion));
        }
    }
}
