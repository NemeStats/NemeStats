using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
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

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayersTests.PlayerCreatorImplTests
{
    [TestFixture]
    public class CreatePlayerTests
    {
        private DataContext dataContextMock;
        private NemeStatsEventTracker eventTrackerMock;
        private PlayerCreatorImpl playerCreator;
        private ApplicationUser currentUser;

        [SetUp]
        public void SetUp()
        {
            dataContextMock = MockRepository.GenerateMock<DataContext>();
            eventTrackerMock = MockRepository.GenerateMock<NemeStatsEventTracker>();
            playerCreator = new PlayerCreatorImpl(dataContextMock, eventTrackerMock);
            currentUser = new ApplicationUser();
        }

        [Test]
        public void ItThrowsAnArgumentNullExceptionIfThePlayerNameIsNull()
        {
            ArgumentNullException expectedException = new ArgumentNullException("playerName");

            Exception exception = Assert.Throws<ArgumentNullException>(() => playerCreator.CreatePlayer(null, currentUser));

            Assert.AreEqual(expectedException.Message, exception.Message);
        }

        [Test]
        public void ItThrowsAnArgumentNullExceptionIfThePlayerNameIsWhitespace()
        {
            string playerName = "    ";
            ArgumentNullException expectedException = new ArgumentNullException("playerName");

            Exception exception = Assert.Throws<ArgumentNullException>(() => playerCreator.CreatePlayer(playerName, currentUser));

            Assert.AreEqual(expectedException.Message, exception.Message);
        }

        [Test]
        public void ItSetsThePlayerName()
        {
            string playerName = "player name";

            playerCreator.CreatePlayer(playerName, currentUser);

            dataContextMock.AssertWasCalled(mock => mock.Save<Player>(
                Arg<Player>.Matches(player => player.Name == playerName), 
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void TheNewPlayerIsActiveWhenCreated()
        {
            playerCreator.CreatePlayer("player name", currentUser);

            dataContextMock.AssertWasCalled(mock => mock.Save<Player>(
                Arg<Player>.Matches(player => player.Active == true),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItRecordsAPlayerCreatedEvent()
        {
            playerCreator.CreatePlayer("player name", currentUser);

            eventTrackerMock.AssertWasCalled(mock => mock.TrackPlayerCreation(currentUser));
        }
    }
}
