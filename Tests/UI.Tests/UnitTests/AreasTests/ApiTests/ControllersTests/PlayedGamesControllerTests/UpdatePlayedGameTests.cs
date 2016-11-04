using System.Net;
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
    public class UpdatePlayedGameTests : ApiControllerTestBase<PlayedGamesController>
    {
        private UpdatedPlayedGameMessage _playedPlayedGameMessage;
        private UpdatedGame _expectedUpdatedGame;

        [SetUp]
        public void LocalSetUp()
        {
            _playedPlayedGameMessage = new UpdatedPlayedGameMessage();

            _expectedUpdatedGame = new UpdatedGame();

            _autoMocker.Get<ITransformer>()
                .Expect(mock => mock.Transform<UpdatedGame>(_playedPlayedGameMessage))
                .IgnoreArguments()
                .Return(_expectedUpdatedGame);
            _autoMocker.Get<IPlayedGameSaver>().Expect(mock => mock.UpdatePlayedGame(
                Arg<UpdatedGame>.Is.Anything,
                Arg<TransactionSource>.Is.Anything,
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void It_Updates_The_Played_Game()
        {
            _autoMocker.ClassUnderTest.UpdatePlayedGame(_playedPlayedGameMessage);

            _autoMocker.Get<IPlayedGameSaver>().AssertWasCalled(mock => mock.UpdatePlayedGame(
                Arg<UpdatedGame>.Is.Same(_expectedUpdatedGame),
                Arg<TransactionSource>.Is.Anything,
                Arg<ApplicationUser>.Is.Same(_applicationUser)));
        }

        [Test]
        public void It_Sets_The_Transaction_Source_To_Rest_API()
        {
            _autoMocker.ClassUnderTest.UpdatePlayedGame(_playedPlayedGameMessage);

            _autoMocker.Get<IPlayedGameSaver>().AssertWasCalled(mock => mock.UpdatePlayedGame(
                Arg<UpdatedGame>.Is.Anything,
                Arg<TransactionSource>.Is.Equal(TransactionSource.RestApi),
                Arg<ApplicationUser>.Is.Anything));
        }

        [Test]
        public void It_Returns_A_204_No_Content_Result()
        {
            var actualResponse = _autoMocker.ClassUnderTest.UpdatePlayedGame(_playedPlayedGameMessage);

            Assert.That(actualResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }
    }
}
