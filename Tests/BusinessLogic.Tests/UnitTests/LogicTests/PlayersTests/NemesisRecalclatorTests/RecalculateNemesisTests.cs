using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.Logic.Nemeses;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Models.Nemeses;
using BusinessLogic.Models;
using BusinessLogic.Models.User;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayersTests.NemesisRecalclatorTests
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
            dataContextMock.Expect(mock => mock.Save<Nemesis>(Arg<Nemesis>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Return(new Nemesis() { Id = newNemesisId });
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
            nemesisList.Add(new Nemesis()
            {
                Id = existingNemesisId,
                NemesisPlayerId = nemesisPlayerId,
                MinionPlayerId = playerId,
                //add 1 so the data is different
                NumberOfGamesLost = gamesLost + 1,
                LossPercentage = lossPercentage
            });
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
            throw new NotImplementedException();
        }

        [Test]
        public void ItReturnsTheUpdatedNemesisIfItWasUpdated()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void ItReturnsTheNewNemesisIfItWasChanged()
        {
            throw new NotImplementedException();
        }
    }
}
