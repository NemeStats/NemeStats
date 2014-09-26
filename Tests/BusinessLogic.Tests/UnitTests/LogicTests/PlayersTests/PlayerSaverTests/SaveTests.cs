using BusinessLogic.DataAccess;
using BusinessLogic.DataAccess.Repositories;
using BusinessLogic.EventTracking;
using BusinessLogic.Logic.Nemeses;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayersTests.PlayerCreatorTests
{
    [TestFixture]
    public class SaveTests
    {
        private IDataContext dataContextMock;
        private NemeStatsEventTracker eventTrackerMock;
        private IPlayerRepository playerRepositoryMock;
        private INemesisRecalculator nemesisRecalculatorMock;
        private PlayerSaver playerSaver;
        private ApplicationUser currentUser;

        [SetUp]
        public void SetUp()
        {
            dataContextMock = MockRepository.GenerateMock<IDataContext>();
            eventTrackerMock = MockRepository.GenerateMock<NemeStatsEventTracker>();
            playerRepositoryMock = MockRepository.GenerateMock<IPlayerRepository>();
            nemesisRecalculatorMock = MockRepository.GenerateMock<INemesisRecalculator>();
            playerSaver = new PlayerSaver(dataContextMock, eventTrackerMock, playerRepositoryMock, nemesisRecalculatorMock);
            currentUser = new ApplicationUser();
        }

        [Test]
        public void ItThrowsAnArgumentNullExceptionIfThePlayerIsNull()
        {
            ArgumentNullException expectedException = new ArgumentNullException("player");

            Exception exception = Assert.Throws<ArgumentNullException>(() => playerSaver.Save(null, currentUser));

            Assert.AreEqual(expectedException.Message, exception.Message);
        }

        [Test]
        public void ItThrowsAnArgumentNullExceptionIfThePlayerNameIsWhitespace()
        {
            Player player = new Player()
            {
                Name = "    "
            };
            ArgumentNullException expectedException = new ArgumentNullException("playerName");

            Exception exception = Assert.Throws<ArgumentNullException>(() => playerSaver.Save(player, currentUser));

            Assert.AreEqual(expectedException.Message, exception.Message);
        }

        [Test]
        public void ItSetsThePlayerName()
        {
            Player player = new Player()
            {
                Name = "player name"
            };

            playerSaver.Save(player, currentUser);

            dataContextMock.AssertWasCalled(mock => mock.Save<Player>(
                Arg<Player>.Matches(savedPlayer => savedPlayer.Name == player.Name), 
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void TheNewPlayerIsActiveWhenCreated()
        {
            playerSaver.Save(new Player() { Name = "player name" }, currentUser);

            dataContextMock.AssertWasCalled(mock => mock.Save<Player>(
                Arg<Player>.Matches(player => player.Active == true),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItRecordsAPlayerCreatedEventIfThePlayerIsNew()
        {
            Player player = MockRepository.GeneratePartialMock<Player>();
            player.Name = "player name";
            player.Expect(mock => mock.AlreadyInDatabase())
                .Return(false);

            playerSaver.Save(player, currentUser);

            eventTrackerMock.AssertWasCalled(mock => mock.TrackPlayerCreation(currentUser));
        }

        [Test]
        public void ItDoesNotRecordsAPlayerCreatedEventIfThePlayerIsNotNew()
        {
            Player player = MockRepository.GeneratePartialMock<Player>();
            player.Name = "player name";
            player.Expect(mock => mock.AlreadyInDatabase())
                .Return(true);

            dataContextMock.Expect(mock => mock.GetQueryable<Player>())
                .Return(new List<Player>().AsQueryable());

            playerSaver.Save(player, currentUser);

            eventTrackerMock.AssertWasNotCalled(mock => mock.TrackPlayerCreation(currentUser));
        }

        [Test]
        public void ItRecalculatesTheNemesisOfTheCurrentPlayersMinionsIfThePlayerIsGoingInactive()
        {
            Player player = MockRepository.GeneratePartialMock<Player>();
            player.Name = "player name";
            player.Active = false;
            player.Id = 151516;
            player.Expect(mock => mock.AlreadyInDatabase())
                .Return(true);

            int expectedPlayerId1 = 1;
            int expectedPlayerId2 = 2;

            Player activeMinion1 = new Player()
            {
                Id = expectedPlayerId1
            };
            Nemesis activeNemesis = new Nemesis()
            { 
                NemesisPlayerId = player.Id,
            };
            activeMinion1.Nemesis = activeNemesis;

            Player activeMinion2 = new Player()
            {
                Id = expectedPlayerId2
            };
            Nemesis secondActiveNemesis = new Nemesis()
            { 
                NemesisPlayerId = player.Id,
            };
            activeMinion2.Nemesis = secondActiveNemesis;

            Player inactiveMinion = new Player()
            {
                Id = -1,
                Active = false
            };
            Nemesis inactiveNemesis = new Nemesis()
            { 
                NemesisPlayerId = player.Id
            };
            inactiveMinion.Nemesis = inactiveNemesis;

            IQueryable<Player> minionPlayers = new List<Player>()
            {
                activeMinion1,
                activeMinion2,
                inactiveMinion
            }.AsQueryable<Player>();

            dataContextMock.Expect(mock => mock.GetQueryable<Player>())
                .Return(minionPlayers);

            playerSaver.Save(player, currentUser);

            nemesisRecalculatorMock.AssertWasCalled(mock => mock.RecalculateNemesis(activeMinion1.Id, currentUser));
            nemesisRecalculatorMock.AssertWasCalled(mock => mock.RecalculateNemesis(activeMinion2.Id, currentUser));
            nemesisRecalculatorMock.AssertWasNotCalled(mock => mock.RecalculateNemesis(inactiveMinion.Id, currentUser));
        }

        [Test]
        public void ItDoesNotRecalculateTheNemesisOfTheCurrentPlayersMinionsIfThePlayerIsStillActive()
        {
            Player player = MockRepository.GeneratePartialMock<Player>();
            player.Name = "player name";
            player.Active = true;
            player.Id = 151516;

            player.Expect(mock => mock.AlreadyInDatabase())
                .Return(true);

            int currentPlayerMinionId1 = 10;
            IQueryable<Nemesis> minionsOfInactivePlayer = new List<Nemesis>()
            {
                new Nemesis(){ NemesisPlayerId = player.Id, MinionPlayerId = currentPlayerMinionId1 },
            }.AsQueryable<Nemesis>();

            dataContextMock.Expect(mock => mock.GetQueryable<Player>())
    .           Return(new List<Player>().AsQueryable());

            playerSaver.Save(player, currentUser);

            nemesisRecalculatorMock.AssertWasNotCalled(mock => mock.RecalculateNemesis(currentPlayerMinionId1, currentUser));
        }
    }
}
