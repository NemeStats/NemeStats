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
        private PlayedGameMessage playedGameMessage;
        private const int EXPECTED_PLAYED_GAME_ID = 1;

        [SetUp]
        public void LocalSetUp()
        {
            playedGameMessage = new PlayedGameMessage
            {
                DatePlayed = "2015-04-10",
                GameDefinitionId = 1,
                Notes = "some notes"
            };

            PlayedGame expectedPlayedGame = new PlayedGame
            {
                Id = EXPECTED_PLAYED_GAME_ID
            };
            autoMocker.Get<IPlayedGameCreator>().Expect(mock => mock.CreatePlayedGame(
                                                                          Arg<NewlyCompletedGame>.Is.Anything,
                                                                          Arg<TransactionSource>.Is.Anything,
                                                                          Arg<ApplicationUser>.Is.Anything))
                    .Return(expectedPlayedGame);
        }

        [Test]
        public void ItRecordsThePlayedGameWithTheTransactionSourceSetToRestApi()
        {
            autoMocker.ClassUnderTest.RecordPlayedGame(playedGameMessage, applicationUser.CurrentGamingGroupId);

            autoMocker.Get<IPlayedGameCreator>().AssertWasCalled(mock => mock.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Is.Anything,
                Arg<TransactionSource>.Is.Equal(TransactionSource.RestApi),
                Arg<ApplicationUser>.Is.Same(applicationUser)));
        }

        [Test]
        public void ItRecordsTheDatePlayed()
        {
            DateTime expectedDateTime = new DateTime(2015, 4, 10);

            autoMocker.ClassUnderTest.RecordPlayedGame(playedGameMessage, applicationUser.CurrentGamingGroupId);

            autoMocker.Get<IPlayedGameCreator>().AssertWasCalled(mock => mock.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Matches(x => x.DatePlayed.Date == expectedDateTime.Date),
                Arg<TransactionSource>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItSetsTheDatePlayedToTodayIfItIsNotSpecified()
        {
            playedGameMessage.DatePlayed = null;

            autoMocker.ClassUnderTest.RecordPlayedGame(playedGameMessage, applicationUser.CurrentGamingGroupId);

            autoMocker.Get<IPlayedGameCreator>().AssertWasCalled(mock => mock.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Matches(x => x.DatePlayed.Date == DateTime.UtcNow.Date),
                Arg<TransactionSource>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItSetsTheGameDefinitionId()
        {
            autoMocker.ClassUnderTest.RecordPlayedGame(playedGameMessage, applicationUser.CurrentGamingGroupId);

            autoMocker.Get<IPlayedGameCreator>().AssertWasCalled(mock => mock.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Matches(x => x.GameDefinitionId == playedGameMessage.GameDefinitionId),
                Arg<TransactionSource>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItSetsTheNotes()
        {
            autoMocker.ClassUnderTest.RecordPlayedGame(playedGameMessage, applicationUser.CurrentGamingGroupId);

            autoMocker.Get<IPlayedGameCreator>().AssertWasCalled(mock => mock.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Matches(x => x.Notes == playedGameMessage.Notes),
                Arg<TransactionSource>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItSetsThePlayerRanks()
        {
            playedGameMessage.PlayerRanks = new List<PlayerRank>
            {
                new PlayerRank() { GameRank = 1, PlayerId = 100, PointsScored = 10 },
                new PlayerRank() { GameRank = 2, PlayerId = 200, PointsScored = 8 }
            };
            autoMocker.ClassUnderTest.RecordPlayedGame(playedGameMessage, applicationUser.CurrentGamingGroupId);
            IList<object[]> arguments = autoMocker.Get<IPlayedGameCreator>().GetArgumentsForCallsMadeOn(mock => mock.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Is.Anything,
                Arg<TransactionSource>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));

            NewlyCompletedGame newlyCompletedGame = arguments[0][0] as NewlyCompletedGame;
            Assert.That(newlyCompletedGame.PlayerRanks[0].GameRank, Is.EqualTo(playedGameMessage.PlayerRanks[0].GameRank));
            Assert.That(newlyCompletedGame.PlayerRanks[0].PlayerId, Is.EqualTo(playedGameMessage.PlayerRanks[0].PlayerId));
            Assert.That(newlyCompletedGame.PlayerRanks[0].PointsScored, Is.EqualTo(playedGameMessage.PlayerRanks[0].PointsScored));
            Assert.That(newlyCompletedGame.PlayerRanks[1].GameRank, Is.EqualTo(playedGameMessage.PlayerRanks[1].GameRank));
            Assert.That(newlyCompletedGame.PlayerRanks[1].PlayerId, Is.EqualTo(playedGameMessage.PlayerRanks[1].PlayerId));
            Assert.That(newlyCompletedGame.PlayerRanks[1].PointsScored, Is.EqualTo(playedGameMessage.PlayerRanks[1].PointsScored));
        }

        [Test]
        public void ItSetsTheGamingGroupIdFromTheRequestIfSpecified()
        {
            int? gamingGroupId = applicationUser.CurrentGamingGroupId + 100;
            playedGameMessage.GamingGroupId = gamingGroupId;

            autoMocker.ClassUnderTest.RecordPlayedGame(playedGameMessage, applicationUser.CurrentGamingGroupId);

            autoMocker.Get<IPlayedGameCreator>().AssertWasCalled(mock => mock.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Matches(x => x.GamingGroupId == gamingGroupId),
                Arg<TransactionSource>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void ItReturnsThePlayedGameIdOfTheNewlyCreatedPlayedGame()
        {
            var actualResponse = autoMocker.ClassUnderTest.RecordPlayedGame(playedGameMessage, applicationUser.CurrentGamingGroupId);

            Assert.That(actualResponse.Content, Is.TypeOf(typeof(ObjectContent<NewlyRecordedPlayedGameMessage>)));
            ObjectContent<NewlyRecordedPlayedGameMessage> content = actualResponse.Content as ObjectContent<NewlyRecordedPlayedGameMessage>;
            NewlyRecordedPlayedGameMessage newlyRecordedPlayedGameMessage = content.Value as NewlyRecordedPlayedGameMessage;
            Assert.That(newlyRecordedPlayedGameMessage.PlayedGameId, Is.EqualTo(EXPECTED_PLAYED_GAME_ID));
        }
    }
}
