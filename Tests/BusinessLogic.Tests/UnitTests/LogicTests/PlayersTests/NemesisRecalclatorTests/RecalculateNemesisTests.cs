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

        [SetUp]
        public void SetUp()
        {
            playerRepositoryMock = MockRepository.GenerateMock<IPlayerRepository>();
            dataContextMock = MockRepository.GenerateMock<IDataContext>();
            nemesisRecalculator = new NemesisRecalculator(dataContextMock, playerRepositoryMock);

            currentUser = new ApplicationUser();
            minionPlayer = new Player()
            {
                NemesisId = 1
            };
            dataContextMock.Expect(mock => mock.FindById<Player>(playerId))
                .Return(minionPlayer);
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

            nemesisRecalculator.RecalculateNemesis(playerId, currentUser);

            dataContextMock.AssertWasCalled(mock => mock.Save<Nemesis>(
                Arg<Nemesis>.Matches(savedNemesis => savedNemesis.MinionPlayerId == playerId
                                        && savedNemesis.NemesisPlayerId == nemesisData.NemesisPlayerId
                                        && savedNemesis.NumberOfGamesLost == nemesisData.NumberOfGamesLost
                                        && savedNemesis.LossPercentage == nemesisData.LossPercentage),
                Arg<ApplicationUser>.Is.Same(currentUser)));
        }
    }
}
