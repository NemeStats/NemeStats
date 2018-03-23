using System;
using System.Threading;
using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
using BusinessLogic.Exceptions;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayersTests.PlayerSaverTests
{
    [TestFixture]
    public class CreatePlayerTests : PlayerSaverTestBase
    {
        private CreatePlayerRequest _createPlayerRequest;

        [SetUp]
        public void SetUp()
        {
            _createPlayerRequest = new CreatePlayerRequest
            {
                Name = "player name"
            };
        }

        [Test]
        public void ItThrowsAnArgumentNullExceptionIfThePlayerIsNull()
        {
            var expectedException = new ArgumentNullException("createPlayerRequest");

            Exception exception = Assert.Throws<ArgumentNullException>(() => _autoMocker.ClassUnderTest.CreatePlayer(null, _currentUser));

            Assert.AreEqual(expectedException.Message, exception.Message);
        }

        [Test]
        public void ItThrowsAnArgumentNullExceptionIfThePlayerNameIsWhitespace()
        {
            _createPlayerRequest.Name = "    ";
            var expectedException = new ArgumentNullException("playerName");

            Exception exception = Assert.Throws<ArgumentNullException>(() => _autoMocker.ClassUnderTest.CreatePlayer(_createPlayerRequest, _currentUser));

            Assert.AreEqual(expectedException.Message, exception.Message);
        }

        [Test]
        public void ItThrowsAUserHasNoGamingGroupExceptionIfTheUserHasNoGamingGroup()
        {
            _currentUser.CurrentGamingGroupId = null;
            var expectedException = new UserHasNoGamingGroupException(_currentUser.Id);

            var actualException = Assert.Throws<UserHasNoGamingGroupException>(() => _autoMocker.ClassUnderTest.CreatePlayer(_createPlayerRequest, _currentUser));

            Assert.AreEqual(expectedException.Message, actualException.Message);
        }

        [Test]
        public void ItThrowsAPlayerWithThisEmailAlreadyExistsExceptionIfAPlayerAlreadyHasTheSpecifiedEmailInTheGamingGroup()
        {
            //--arrange
            _createPlayerRequest.PlayerEmailAddress = _playerWithRegisteredUser.User.Email;
            var expectedException = new PlayerWithThisEmailAlreadyExistsException(_createPlayerRequest.PlayerEmailAddress, _playerWithRegisteredUser.Name, _playerWithRegisteredUser.Id);

            //--act
            var actualException = Assert.Throws<PlayerWithThisEmailAlreadyExistsException>(() => _autoMocker.ClassUnderTest.CreatePlayer(_createPlayerRequest, _currentUser));

            //--assert
            actualException.Message.ShouldBe(expectedException.Message);
        }

        [Test]
        public void ItSetsThePlayerName()
        {
            _autoMocker.ClassUnderTest.CreatePlayer(_createPlayerRequest, _currentUser);

            _autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                Arg<Player>.Matches(savedPlayer => savedPlayer.Name == _createPlayerRequest.Name),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void TheNewPlayerIsActiveWhenCreated()
        {
            _createPlayerRequest.Name = "player name";
            _autoMocker.ClassUnderTest.CreatePlayer(_createPlayerRequest, _currentUser);

            _autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                Arg<Player>.Matches(player => player.Active),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItAssociatesThePlayerWithTheCurrentUserIfRequested()
        {
            _createPlayerRequest.Name = "player name";
            _autoMocker.ClassUnderTest.CreatePlayer(_createPlayerRequest, _currentUser, true);

            _autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                Arg<Player>.Matches(player => player.ApplicationUserId == _currentUser.Id),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItAssociatesThePlayerWithTheSpecifiedGamingGroup()
        {
            //--arrange
            _createPlayerRequest.GamingGroupId = 53;

            //--act
            _autoMocker.ClassUnderTest.CreatePlayer(_createPlayerRequest, _currentUser);

            //--assert
            _autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                Arg<Player>.Matches(savedPlayer => savedPlayer.GamingGroupId == _createPlayerRequest.GamingGroupId),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItUsesTheCurrentPlayersGamingGroupIdIfTheUserDidntSpecifyOne()
        {
            //--arrange
            _createPlayerRequest.GamingGroupId = 1;

            //--act
            _autoMocker.ClassUnderTest.CreatePlayer(_createPlayerRequest, _currentUser);

            //--assert
            _autoMocker.Get<IDataContext>().AssertWasCalled(mock => mock.Save(
                Arg<Player>.Matches(savedPlayer => savedPlayer.GamingGroupId == _createPlayerRequest.GamingGroupId),
                Arg<ApplicationUser>.Is.Anything));
        }


        [Test, Ignore("This test is too flaky due to the Task taking an indeterminate amount of time")]
        public void ItRecordsAPlayerCreatedEvent()
        {
            _autoMocker.ClassUnderTest.CreatePlayer(_createPlayerRequest, _currentUser);

            try
            {
                _autoMocker.Get<INemeStatsEventTracker>().AssertWasCalled(mock => mock.TrackPlayerCreation(_currentUser));
            }
            catch (Exception)
            {
                //since this happens in a task there can be a race condition where the test runs before this method is called. Hopefully this
                // solves the problem
                Thread.Sleep(200);
                _autoMocker.Get<INemeStatsEventTracker>().AssertWasCalled(mock => mock.TrackPlayerCreation(_currentUser));

            }
        }

        [Test]
        public void ItThrowsAPlayerAlreadyExistsExceptionIfAttemptingToSaveAPlayerWithANameThatAlreadyExists()
        {
            _createPlayerRequest.Name = _playerThatAlreadyExists.Name;

            var exception = Assert.Throws<PlayerAlreadyExistsException>(
                () => _autoMocker.ClassUnderTest.CreatePlayer(_createPlayerRequest, _currentUser));

            Assert.AreEqual(_idOfPlayerThatAlreadyExists, exception.ExistingPlayerId);
        }
    }
}
