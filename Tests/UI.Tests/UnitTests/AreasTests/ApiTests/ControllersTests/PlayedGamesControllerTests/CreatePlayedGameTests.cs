using BusinessLogic.Logic;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Net.Http;
using UI.Areas.Api.Controllers;
using UI.Areas.Api.Models;

namespace UI.Tests.UnitTests.AreasTests.ApiTests.ControllersTests.PlayedGamesControllerTests
{
    [TestFixture]
    public class PlayedGamesControllerTests : ApiControllerTestBase<PlayedGamesController>
    {
        private PlayedGameMessage _playedGameMessage;
        private const int EXPECTED_PLAYED_GAME_ID = 1;
        private const int EXPECTED_GAMING_GROUP_ID = 183;

        [SetUp]
        public void LocalSetUp()
        {
            _playedGameMessage = new PlayedGameMessage
            {
                DatePlayed = "2015-04-10",
                GameDefinitionId = 1,
                Notes = "some notes"
            };

            var expectedPlayedGame = new PlayedGame
            {
                Id = EXPECTED_PLAYED_GAME_ID,
                GamingGroupId = EXPECTED_GAMING_GROUP_ID
            };
            _autoMocker.Get<IPlayedGameCreator>().Expect(mock => mock.CreatePlayedGame(
                                                                          Arg<NewlyCompletedGame>.Is.Anything,
                                                                          Arg<TransactionSource>.Is.Anything,
                                                                          Arg<ApplicationUser>.Is.Anything))
                    .Return(expectedPlayedGame);
        }

        [Test]
        public void ItRecordsThePlayedGameWithTheTransactionSourceSetToRestApi()
        {
            _autoMocker.ClassUnderTest.RecordPlayedGame(_playedGameMessage, _applicationUser.CurrentGamingGroupId);

            _autoMocker.Get<IPlayedGameCreator>().AssertWasCalled(mock => mock.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Is.Anything,
                Arg<TransactionSource>.Is.Equal(TransactionSource.RestApi),
                Arg<ApplicationUser>.Is.Same(_applicationUser)));
        }

        [Test]
        public void ItRecordsTheDatePlayed()
        {
            var expectedDateTime = new DateTime(2015, 4, 10);

            _autoMocker.ClassUnderTest.RecordPlayedGame(_playedGameMessage, _applicationUser.CurrentGamingGroupId);

            _autoMocker.Get<IPlayedGameCreator>().AssertWasCalled(mock => mock.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Matches(x => x.DatePlayed.Date == expectedDateTime.Date),
                Arg<TransactionSource>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItSetsTheDatePlayedToTodayIfItIsNotSpecified()
        {
            _playedGameMessage.DatePlayed = null;

            _autoMocker.ClassUnderTest.RecordPlayedGame(_playedGameMessage, _applicationUser.CurrentGamingGroupId);

            _autoMocker.Get<IPlayedGameCreator>().AssertWasCalled(mock => mock.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Matches(x => x.DatePlayed.Date == DateTime.UtcNow.Date),
                Arg<TransactionSource>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItSetsTheGameDefinitionId()
        {
            _autoMocker.ClassUnderTest.RecordPlayedGame(_playedGameMessage, _applicationUser.CurrentGamingGroupId);

            _autoMocker.Get<IPlayedGameCreator>().AssertWasCalled(mock => mock.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Matches(x => x.GameDefinitionId == _playedGameMessage.GameDefinitionId),
                Arg<TransactionSource>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItSetsTheNotes()
        {
            _autoMocker.ClassUnderTest.RecordPlayedGame(_playedGameMessage, _applicationUser.CurrentGamingGroupId);

            _autoMocker.Get<IPlayedGameCreator>().AssertWasCalled(mock => mock.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Matches(x => x.Notes == _playedGameMessage.Notes),
                Arg<TransactionSource>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItSetsThePlayerRanks()
        {
            _playedGameMessage.PlayerRanks = new List<PlayerRank>
            {
                new PlayerRank() { GameRank = 1, PlayerId = 100, PointsScored = 10 },
                new PlayerRank() { GameRank = 2, PlayerId = 200, PointsScored = 8 }
            };
            _autoMocker.ClassUnderTest.RecordPlayedGame(_playedGameMessage, _applicationUser.CurrentGamingGroupId);
            var arguments = _autoMocker.Get<IPlayedGameCreator>().GetArgumentsForCallsMadeOn(mock => mock.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Is.Anything,
                Arg<TransactionSource>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));

            var newlyCompletedGame = arguments[0][0] as NewlyCompletedGame;
            Assert.That(newlyCompletedGame.PlayerRanks[0].GameRank, Is.EqualTo(_playedGameMessage.PlayerRanks[0].GameRank));
            Assert.That(newlyCompletedGame.PlayerRanks[0].PlayerId, Is.EqualTo(_playedGameMessage.PlayerRanks[0].PlayerId));
            Assert.That(newlyCompletedGame.PlayerRanks[0].PointsScored, Is.EqualTo(_playedGameMessage.PlayerRanks[0].PointsScored));
            Assert.That(newlyCompletedGame.PlayerRanks[1].GameRank, Is.EqualTo(_playedGameMessage.PlayerRanks[1].GameRank));
            Assert.That(newlyCompletedGame.PlayerRanks[1].PlayerId, Is.EqualTo(_playedGameMessage.PlayerRanks[1].PlayerId));
            Assert.That(newlyCompletedGame.PlayerRanks[1].PointsScored, Is.EqualTo(_playedGameMessage.PlayerRanks[1].PointsScored));
        }

        [Test]
        public void ItSetsTheGamingGroupIdFromTheRequestIfSpecified()
        {
            int? gamingGroupId = _applicationUser.CurrentGamingGroupId + 100;
            _playedGameMessage.GamingGroupId = gamingGroupId;

            _autoMocker.ClassUnderTest.RecordPlayedGame(_playedGameMessage, _applicationUser.CurrentGamingGroupId);

            _autoMocker.Get<IPlayedGameCreator>().AssertWasCalled(mock => mock.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Matches(x => x.GamingGroupId == gamingGroupId),
                Arg<TransactionSource>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItReturnsThePlayedGameIdAndGamingGroupIdOfTheNewlyCreatedPlayedGame()
        {
            var actualResponse = _autoMocker.ClassUnderTest.RecordPlayedGame(_playedGameMessage, _applicationUser.CurrentGamingGroupId);

            Assert.That(actualResponse.Content, Is.TypeOf(typeof(ObjectContent<NewlyRecordedPlayedGameMessage>)));
            var content = actualResponse.Content as ObjectContent<NewlyRecordedPlayedGameMessage>;
            var newlyRecordedPlayedGameMessage = content.Value as NewlyRecordedPlayedGameMessage;
            Assert.That(newlyRecordedPlayedGameMessage.PlayedGameId, Is.EqualTo(EXPECTED_PLAYED_GAME_ID));
            Assert.That(newlyRecordedPlayedGameMessage.GamingGroupId, Is.EqualTo(EXPECTED_GAMING_GROUP_ID));
        }
    }
}
