using System.Collections.Generic;
using BusinessLogic.DataAccess;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.PlayedGames;
using NUnit.Framework;
using Rhino.Mocks;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayedGamesTests.PlayedGameSaverTests
{
    [TestFixture]
    public class CreateApplicationLinkagesTests : PlayedGameSaverTestBase
    {
        [Test]
        public void ItCreatesAnApplicationLinkageForNemeStats()
        {
            //--arrange
            int playedGameId = 1;
            var dataContext = MockRepository.GenerateMock<IDataContext>();

            //--act
            AutoMocker.ClassUnderTest.CreateApplicationLinkages(new List<ApplicationLinkage>(), playedGameId, dataContext);

            //--assert
            AutoMocker.Get<IApplicationLinker>().AssertWasCalled(mock => mock.LinkApplication(
                playedGameId,
                ApplicationLinker.APPLICATION_NAME_NEMESTATS,
                playedGameId.ToString(),
                dataContext));
        }

        [Test]
        public void ItCreatesAnApplicationLinkageForEachSpecifiedApplicationLinkage()
        {
            //--arrange
            var expectedApplicationLinkage1 = new ApplicationLinkage
            {
                ApplicationName = "app1",
                EntityId = "1"
            };
            var expectedApplicationLinkage2 = new ApplicationLinkage
            {
                ApplicationName = "app2",
                EntityId = "2"
            };
            var applicationLinkages = new List<ApplicationLinkage>
            {
                expectedApplicationLinkage1,
                expectedApplicationLinkage2
            };

            var expectedPlayedGame = new PlayedGame
            {
                Id = EXPECTED_PLAYED_GAME_ID
            };
            AutoMocker.Get<IPlayedGameSaver>().Expect(partialMock => partialMock.TransformNewlyCompletedGameIntoPlayedGame(null, 0, null, null))
                .IgnoreArguments()
                .Return(expectedPlayedGame);

            var dataContext = MockRepository.GenerateMock<IDataContext>();

            //--act
            AutoMocker.ClassUnderTest.CreateApplicationLinkages(applicationLinkages, expectedPlayedGame.Id, dataContext);

            //--assert
            AutoMocker.Get<IApplicationLinker>().AssertWasCalled(mock => mock.LinkApplication(
                EXPECTED_PLAYED_GAME_ID,
                expectedApplicationLinkage1.ApplicationName,
                expectedApplicationLinkage1.EntityId,
                dataContext));

            AutoMocker.Get<IApplicationLinker>().AssertWasCalled(mock => mock.LinkApplication(
              EXPECTED_PLAYED_GAME_ID,
              expectedApplicationLinkage2.ApplicationName,
              expectedApplicationLinkage2.EntityId,
                dataContext));
        }

    }
}
