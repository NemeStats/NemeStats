using System.Collections.Generic;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.PlayedGames;
using NemeStats.TestingHelpers.NemeStatsTestingExtensions;
using NUnit.Framework;
using Rhino.Mocks;
using Shouldly;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayedGamesTests.PlayedGameSaverTests
{
    [TestFixture]
    public class TransformNewlyCompletedGameIntoPlayedGameTests : PlayedGameSaverTestBase
    {
        [Test]
        public void It_Sets_The_WinnerType_Based_On_The_Ranks_Of_The_Players()
        {
            //--arrange
            var playerGameResults = new List<PlayerGameResult>
            {
                new PlayerGameResult
                {
                    GameRank = 1
                },
                new PlayerGameResult
                {
                    GameRank = 2
                }
            };
            var expectedWinnerType = WinnerTypes.TeamLoss;
            AutoMocker.Get<IWinnerTypeCalculator>()
                .Expect(mock => mock.CalculateWinnerType(Arg<IList<int>>.Is.Anything)).Return(expectedWinnerType);

            //--act
            var result = AutoMocker.ClassUnderTest.TransformNewlyCompletedGameIntoPlayedGame(new NewlyCompletedGame(), GAMING_GROUP_ID, CurrentUser.Id, playerGameResults);

            //--assert
            var arguments = AutoMocker.Get<IWinnerTypeCalculator>()
                .GetArgumentsForCallsMadeOn(mock => mock.CalculateWinnerType(Arg<IList<int>>.Is.Anything));
            var gameRanks = arguments.AssertFirstCallIsType<IList<int>>();
            gameRanks.ShouldContain(playerGameResults[0].GameRank);
            gameRanks.ShouldContain(playerGameResults[1].GameRank);
            result.WinnerType.ShouldBe(expectedWinnerType);
        }
    }
}
