using BusinessLogic.Logic;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Net.Http;
using UI.Areas.Api.Controllers;
using UI.Areas.Api.Models;
using UI.Transformations;

namespace UI.Tests.UnitTests.AreasTests.ApiTests.ControllersTests.PlayedGamesControllerTests
{
    [TestFixture]
    public class RecordPlayedGameTests : ApiControllerTestBase<PlayedGamesController>
    {
        private PlayedGameMessage _playedGameMessage;
        private const int EXPECTED_PLAYED_GAME_ID = 1;
        private NewlyCompletedGame _expectedNewlyCompletedGame;

        [SetUp]
        public void LocalSetUp()
        {
            _playedGameMessage = new PlayedGameMessage();

            _expectedNewlyCompletedGame = new NewlyCompletedGame();

            _autoMocker.Get<ITransformer>()
                .Expect(mock => mock.Transform<NewlyCompletedGame>(_playedGameMessage))
                .IgnoreArguments()
                .Return(_expectedNewlyCompletedGame);

            var expectedPlayedGame = new PlayedGame
            {
                Id = EXPECTED_PLAYED_GAME_ID,
                GamingGroupId = EXPECTED_GAMING_GROUP_ID
            };
            _autoMocker.Get<IPlayedGameSaver>().Expect(mock => mock.CreatePlayedGame(
                                                                          Arg<NewlyCompletedGame>.Is.Anything,
                                                                          Arg<TransactionSource>.Is.Anything,
                                                                          Arg<ApplicationUser>.Is.Anything))
                    .Return(expectedPlayedGame);
        }

        [Test]
        public void ItRecordsThePlayedGame()
        {
            _autoMocker.ClassUnderTest.RecordPlayedGame(_playedGameMessage, _applicationUser.CurrentGamingGroupId);

            _autoMocker.Get<IPlayedGameSaver>().AssertWasCalled(mock => mock.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Is.Same(_expectedNewlyCompletedGame),
                Arg<TransactionSource>.Is.Anything,
                Arg<ApplicationUser>.Is.Same(_applicationUser)));
        }

        [Test]
        public void ItRecordsThePlayedGameWithTheTransactionSourceSetToRestApi()
        {
            _autoMocker.ClassUnderTest.RecordPlayedGame(_playedGameMessage, _applicationUser.CurrentGamingGroupId);

            _autoMocker.Get<IPlayedGameSaver>().AssertWasCalled(mock => mock.CreatePlayedGame(
                Arg<NewlyCompletedGame>.Is.Anything,
                Arg<TransactionSource>.Is.Equal(TransactionSource.RestApi),
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
