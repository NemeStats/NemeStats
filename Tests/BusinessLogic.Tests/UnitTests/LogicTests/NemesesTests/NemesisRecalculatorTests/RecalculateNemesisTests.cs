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
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.Nemeses;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Models.Nemeses;
using BusinessLogic.Models;
using BusinessLogic.Models.User;

namespace BusinessLogic.Tests.UnitTests.LogicTests.NemesesTests.NemesisRecalclatorTests
{
    [TestFixture]
    public class RecalculateNemesisTests
    {
        private IPlayerRepository playerRepositoryMock;
        private IDataContext dataContextMock;
        private NemesisRecalculator nemesisRecalculator;
        private ApplicationUser currentUser;

        private int playerId = 1;
        private Player minionPlayer;
        private int existingNemesisId = 15153;
        private int newNemesisId = 9999;
        private Nemesis savedNemesis;

        [SetUp]
        public void SetUp()
        {
            playerRepositoryMock = MockRepository.GenerateMock<IPlayerRepository>();
            dataContextMock = MockRepository.GenerateMock<IDataContext>();
            nemesisRecalculator = new NemesisRecalculator(dataContextMock, playerRepositoryMock);

            currentUser = new ApplicationUser();
            minionPlayer = new Player()
            {
                NemesisId = existingNemesisId
            };
            dataContextMock.Expect(mock => mock.FindById<Player>(playerId))
                .Return(minionPlayer);
            savedNemesis = new Nemesis() { Id = newNemesisId };
            dataContextMock.Expect(mock => mock.Save<Nemesis>(Arg<Nemesis>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Return(savedNemesis);
        }

        [Test]
        public void ItReturnsANullNemesisIfTheNewNemesisIsNull()
        {
            playerRepositoryMock.Expect(mock => mock.GetNemesisData(playerId))
                                        .Return(new NullNemesisData());

            Nemesis nullNemesis = nemesisRecalculator.RecalculateNemesis(playerId, currentUser);

            Assert.True(nullNemesis is NullNemesis);
        }

        [Test]
        public void ItClearsTheNemesisIfTheNewNemesisIsNullAndOneAlreadyExisted()
        {
            playerRepositoryMock.Expect(mock => mock.GetNemesisData(playerId))
                                        .Return(new NullNemesisData());

            nemesisRecalculator.RecalculateNemesis(playerId, currentUser);

            dataContextMock.AssertWasCalled(mock => mock.Save<Player>(
                Arg<Player>.Matches(player => player.NemesisId == null), 
                Arg<ApplicationUser>.Is.Equal(currentUser)));
        }

        [Test]
        public void ItSetsTheNewNemesisIfItChanged()
        {
            NemesisData nemesisData = new NemesisData() { NemesisPlayerId = -1 };
            playerRepositoryMock.Expect(mock => mock.GetNemesisData(playerId))
                            .Return(nemesisData);

            dataContextMock.Expect(mock => mock.GetQueryable<Nemesis>())
                .Return(new List<Nemesis>().AsQueryable());

            nemesisRecalculator.RecalculateNemesis(playerId, currentUser);

            dataContextMock.AssertWasCalled(mock => mock.Save<Nemesis>(
                Arg<Nemesis>.Matches(savedNemesis => savedNemesis.MinionPlayerId == playerId
                                        && savedNemesis.NemesisPlayerId == nemesisData.NemesisPlayerId
                                        && savedNemesis.NumberOfGamesLost == nemesisData.NumberOfGamesLost
                                        && savedNemesis.LossPercentage == nemesisData.LossPercentage),
                Arg<ApplicationUser>.Is.Same(currentUser)));
            dataContextMock.AssertWasCalled(mock => mock.Save<Player>(
                Arg<Player>.Matches(player => player.NemesisId == newNemesisId), Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public void ItSetsThePreviousNemesisIfTheCurrentOneChanges()
        {
            NemesisData nemesisData = new NemesisData() { NemesisPlayerId = -1 };
            playerRepositoryMock.Expect(mock => mock.GetNemesisData(playerId))
                            .Return(nemesisData);

            dataContextMock.Expect(mock => mock.GetQueryable<Nemesis>())
                .Return(new List<Nemesis>().AsQueryable());

            nemesisRecalculator.RecalculateNemesis(playerId, currentUser);

            dataContextMock.AssertWasCalled(mock => mock.Save<Player>(
                Arg<Player>.Matches(player => player.PreviousNemesisId == existingNemesisId), Arg<ApplicationUser>.Is.Same(currentUser)));
        }

        [Test]
        public void ItSetsThePreviousNemesisIfTheCurrentOneIsCleared()
        {
            playerRepositoryMock.Expect(mock => mock.GetNemesisData(playerId))
                            .Return(new NullNemesisData());

            nemesisRecalculator.RecalculateNemesis(playerId, currentUser);

            dataContextMock.AssertWasCalled(mock => mock.Save<Player>(
                Arg<Player>.Matches(player => player.PreviousNemesisId == existingNemesisId),
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
            playerRepositoryMock.Expect(mock => mock.GetNemesisData(playerId))
                            .Return(nemesisData);

            List<Nemesis> nemesisList = new List<Nemesis>();
            nemesisList.Add(new Nemesis() 
                                { 
                                    Id = existingNemesisId,
                                    NemesisPlayerId = nemesisPlayerId,
                                    MinionPlayerId = playerId,
                                    NumberOfGamesLost = gamesLost,
                                    LossPercentage = lossPercentage
                                });
            dataContextMock.Expect(mock => mock.GetQueryable<Nemesis>())
                .Return(nemesisList.AsQueryable());

            nemesisRecalculator.RecalculateNemesis(playerId, currentUser);

            dataContextMock.AssertWasNotCalled(mock => mock.Save<Nemesis>(
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
            playerRepositoryMock.Expect(mock => mock.GetNemesisData(playerId))
                            .Return(nemesisData);

            List<Nemesis> nemesisList = new List<Nemesis>();
            Nemesis existingNemesis = new Nemesis()
            {
                Id = existingNemesisId,
                NemesisPlayerId = nemesisPlayerId,
                MinionPlayerId = playerId,
                //add 1 so the data is different
                NumberOfGamesLost = gamesLost + 1,
                LossPercentage = lossPercentage
            };
            nemesisList.Add(existingNemesis);
            dataContextMock.Expect(mock => mock.GetQueryable<Nemesis>())
                .Return(nemesisList.AsQueryable());

            nemesisRecalculator.RecalculateNemesis(playerId, currentUser);

            dataContextMock.AssertWasCalled(mock => mock.Save<Nemesis>(
                Arg<Nemesis>.Matches(nem => nem.Id == existingNemesisId
                                        && nem.MinionPlayerId == playerId
                                        && nem.NemesisPlayerId == nemesisPlayerId
                                        && nem.NumberOfGamesLost == nemesisData.NumberOfGamesLost
                                        && nem.LossPercentage == nemesisData.LossPercentage),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItReturnsTheExistingNemesisIfNothingChanged()
        {
            NemesisData nemesisData = new NemesisData();
            playerRepositoryMock.Expect(mock => mock.GetNemesisData(playerId))
                            .Return(nemesisData);

            List<Nemesis> nemesisList = new List<Nemesis>();
            Nemesis existingNemesis = new Nemesis()
            {
                Id = existingNemesisId,
                MinionPlayerId = playerId
            };
            nemesisList.Add(existingNemesis);
            dataContextMock.Expect(mock => mock.GetQueryable<Nemesis>())
                .Return(nemesisList.AsQueryable());

            Nemesis actualNemesis = nemesisRecalculator.RecalculateNemesis(playerId, currentUser);

            Assert.AreSame(existingNemesis, actualNemesis);
        }

        [Test]
        public void ItReturnsTheUpdatedNemesisIfItWasUpdated()
        {
            int expectedLossPercentage = 15;
            NemesisData nemesisData = new NemesisData() { LossPercentage = expectedLossPercentage };
            playerRepositoryMock.Expect(mock => mock.GetNemesisData(playerId))
                            .Return(nemesisData);

            List<Nemesis> nemesisList = new List<Nemesis>();
            Nemesis existingNemesis = new Nemesis()
            {
                Id = existingNemesisId,
                MinionPlayerId = playerId
                
            };
            nemesisList.Add(existingNemesis);
            dataContextMock.Expect(mock => mock.GetQueryable<Nemesis>())
                .Return(nemesisList.AsQueryable());

            Nemesis actualNemesis = nemesisRecalculator.RecalculateNemesis(playerId, currentUser);

            Assert.AreSame(savedNemesis, actualNemesis);
        }

        [Test]
        public void ItReturnsTheNewNemesisIfItWasChanged()
        {
            //change the nemesis
            NemesisData nemesisData = new NemesisData() { NemesisPlayerId = 19383 };
            playerRepositoryMock.Expect(mock => mock.GetNemesisData(playerId))
                            .Return(nemesisData);

            List<Nemesis> nemesisList = new List<Nemesis>();
            Nemesis existingNemesis = new Nemesis()
            {
                Id = existingNemesisId,
                MinionPlayerId = playerId

            };
            nemesisList.Add(existingNemesis);
            dataContextMock.Expect(mock => mock.GetQueryable<Nemesis>())
                .Return(nemesisList.AsQueryable());

            Nemesis actualNemesis = nemesisRecalculator.RecalculateNemesis(playerId, currentUser);

            Assert.AreSame(savedNemesis, actualNemesis);
        }
    }
}
