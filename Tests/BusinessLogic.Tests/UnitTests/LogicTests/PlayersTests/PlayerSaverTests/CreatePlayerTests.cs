using System;
using System.Threading;
using BusinessLogic.DataAccess;
using BusinessLogic.EventTracking;
using BusinessLogic.Exceptions;
using BusinessLogic.Logic.Players;
using BusinessLogic.Models;
using BusinessLogic.Models.Players;
using BusinessLogic.Models.User;
using NemeStats.TestingHelpers.NemeStatsTestingExtensions;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayersTests.PlayerSaverTests
{
    [TestFixture]
    public class CreatePlayerTests : PlayerSaverTestBase
    {
        private CreatePlayerRequest _createPlayerRequest;
        private Player _expectedSavedPlayer;

        [SetUp]
        public void SetUp()
        {
            _createPlayerRequest = new CreatePlayerRequest
            {
                Name = "player name",
                GamingGroupId = _currentUser.CurrentGamingGroupId.Value
            };

            _expectedSavedPlayer = new Player
            {
                Name = _createPlayerRequest.Name,
                Id = 89,
                GamingGroupId = _currentUser.CurrentGamingGroupId.Value
            };
            _autoMocker.Get<IDataContext>()
                .Expect(mock => mock.Save(Arg<Player>.Is.Anything, Arg<ApplicationUser>.Is.Anything))
                .Return(_expectedSavedPlayer);
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
        public void ItThrowsANoValidGamingGroupExceptionIfTheUserHasNoGamingGroupAndNoneWasSpecified()
        {
            _currentUser.CurrentGamingGroupId = null;
            _createPlayerRequest.GamingGroupId = null;
            var expectedException = new NoValidGamingGroupException(_currentUser.Id);

            var actualException = Assert.Throws<NoValidGamingGroupException>(() => _autoMocker.ClassUnderTest.CreatePlayer(_createPlayerRequest, _currentUser));

            Assert.AreEqual(expectedException.Message, actualException.Message);
        }

        [Test]
        public void ItThrowsAnArgumentExceptionIfBothAnEmailAddressIsEnteredAndTheFlagIsSetToAssociateThePlayerWithTheCurrentUser()
        {
            //--arrange
            _createPlayerRequest.PlayerEmailAddress = "some email";
            var expectedException = new ArgumentException("You cannot specify an email address for the new Player while simultaneously requesting to associate the Player with the current user.");

            //--act
            var actualException = Assert.Throws<ArgumentException>(() => _autoMocker.ClassUnderTest.CreatePlayer(_createPlayerRequest, _currentUser, true));

            //--assert
            actualException.Message.ShouldBe(expectedException.Message);
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
        public void ItInvitesAnotherUserIfTheEmailAddressWasSpecified()
        {
            //--arrange
            _createPlayerRequest.PlayerEmailAddress = "some email";

            //--act
            _autoMocker.ClassUnderTest.CreatePlayer(_createPlayerRequest, _currentUser);

            //--assert
            var args = _autoMocker.Get<IPlayerInviter>().GetArgumentsForCallsMadeOn(mock =>
                mock.InvitePlayer(Arg<PlayerInvitation>.Is.Anything, Arg<ApplicationUser>.Is.Anything));
            var actualPlayerInvitation = args.AssertFirstCallIsType<PlayerInvitation>();
            actualPlayerInvitation.CustomEmailMessage.ShouldBeNull();
            actualPlayerInvitation.EmailSubject.ShouldBe("NemeStats Invitation from " + _currentUser.UserName);
            actualPlayerInvitation.InvitedPlayerEmail.ShouldBe(_createPlayerRequest.PlayerEmailAddress);
            actualPlayerInvitation.InvitedPlayerId.ShouldBe(_expectedSavedPlayer.Id);
            actualPlayerInvitation.GamingGroupId.ShouldBe(_expectedSavedPlayer.GamingGroupId);

            var actualApplicationUser = args.AssertFirstCallIsType<ApplicationUser>(1);
            actualApplicationUser.ShouldBeSameAs(_currentUser);
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
