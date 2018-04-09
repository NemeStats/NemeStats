using BusinessLogic.Logic;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.User;
using NUnit.Framework;
using Rhino.Mocks;
using System.Net.Http;
using NemeStats.TestingHelpers.NemeStatsTestingExtensions;
using Shouldly;
using UI.Areas.Api.Controllers;
using UI.Areas.Api.Models;
using UI.HtmlHelpers;

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
            _autoMocker.Get<ICreatePlayedGameComponent>().Expect(mock => mock.Execute(
                                                                          Arg<NewlyCompletedGame>.Is.Anything,
                                                                          Arg<ApplicationUser>.Is.Anything))
                    .Return(expectedPlayedGame);
        }

        [Test]
        public void ItRecordsThePlayedGameWithTheTransactionSourceSetToRestApi()
        {
            _autoMocker.ClassUnderTest.RecordPlayedGame(_playedGameMessage, EXPECTED_GAMING_GROUP_ID);

            var arguments = _autoMocker.Get<ICreatePlayedGameComponent>().GetArgumentsForCallsMadeOn(mock => mock.Execute(
                Arg<NewlyCompletedGame>.Is.Anything,
                Arg<ApplicationUser>.Is.Same(_applicationUser)));

            var actualNewlyCompletedGame = arguments.AssertFirstCallIsType<NewlyCompletedGame>(0);
            actualNewlyCompletedGame.ShouldBeSameAs(_expectedNewlyCompletedGame);
            actualNewlyCompletedGame.TransactionSource.ShouldBe(TransactionSource.RestApi);
        }

        [Test]
        public void ItReturnsThePlayedGameIdAndGamingGroupIdOfTheNewlyCreatedPlayedGame()
        {
            var expectedNemeStatsUrl = AbsoluteUrlBuilder.GetPlayedGameDetailsUrl(EXPECTED_PLAYED_GAME_ID);

            var actualResponse = _autoMocker.ClassUnderTest.RecordPlayedGame(_playedGameMessage, EXPECTED_GAMING_GROUP_ID);

            Assert.That(actualResponse.Content, Is.TypeOf(typeof(ObjectContent<NewlyRecordedPlayedGameMessage>)));
            var content = actualResponse.Content as ObjectContent<NewlyRecordedPlayedGameMessage>;
            var newlyRecordedPlayedGameMessage = content.Value as NewlyRecordedPlayedGameMessage;
            Assert.That(newlyRecordedPlayedGameMessage.PlayedGameId, Is.EqualTo(EXPECTED_PLAYED_GAME_ID));
            Assert.That(newlyRecordedPlayedGameMessage.GamingGroupId, Is.EqualTo(EXPECTED_GAMING_GROUP_ID));
            Assert.That(newlyRecordedPlayedGameMessage.NemeStatsUrl, Is.EqualTo(expectedNemeStatsUrl));
        }
    }
}
