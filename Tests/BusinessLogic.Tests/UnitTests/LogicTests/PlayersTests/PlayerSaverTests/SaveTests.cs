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

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayersTests.PlayerCreatorTests
{
    [TestFixture]
    public class SaveTests
    {
        private IDataContext dataContextMock;
        private NemeStatsEventTracker eventTrackerMock;
        private PlayerSaver playerSaver;
        private ApplicationUser currentUser;

        [SetUp]
        public void SetUp()
        {
            dataContextMock = MockRepository.GenerateMock<IDataContext>();
            eventTrackerMock = MockRepository.GenerateMock<NemeStatsEventTracker>();
            playerSaver = new PlayerSaver(dataContextMock, eventTrackerMock);
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

            playerSaver.Save(player, currentUser);

            eventTrackerMock.AssertWasNotCalled(mock => mock.TrackPlayerCreation(currentUser));
        }
    }
}
