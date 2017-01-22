using System.Collections.Generic;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.PlayedGames;
using NUnit.Framework;
using Shouldly;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayedGamesTests.PlayedGameSaverTests
{
    [TestFixture]
    public class TransformNewlyCompletedGameIntoPlayedGameTests : PlayedGameSaverTestBase
    {
        [Test]
        public void It_Sets_The_WinnerType_To_Team_Win_If_All_Players_Won()
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
                    GameRank = 1
                }
            };

            //--act
            var result = AutoMocker.ClassUnderTest.TransformNewlyCompletedGameIntoPlayedGame(new NewlyCompletedGame(), GAMING_GROUP_ID, CurrentUser.Id, playerGameResults);

            //--assert
            result.WinnerType.ShouldBe(WinnerTypes.TeamWin);

        }

        [Test]
        public void It_Sets_The_WinnerType_To_Team_Loss_If_All_Players_Lost()
        {
            //--arrange
            var playerGameResults = new List<PlayerGameResult>
            {
                new PlayerGameResult
                {
                    GameRank = 2
                },
                new PlayerGameResult
                {
                    GameRank = 2
                }
            };

            //--act
            var result = AutoMocker.ClassUnderTest.TransformNewlyCompletedGameIntoPlayedGame(new NewlyCompletedGame(), GAMING_GROUP_ID, CurrentUser.Id, playerGameResults);

            //--assert
            result.WinnerType.ShouldBe(WinnerTypes.TeamLoss);

        }

        [Test]
        public void It_Sets_The_WinnerType_To_Player_Win_If_There_Was_At_Least_One_Winner_And_One_Loser()
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

            //--act
            var result = AutoMocker.ClassUnderTest.TransformNewlyCompletedGameIntoPlayedGame(new NewlyCompletedGame(), GAMING_GROUP_ID, CurrentUser.Id, playerGameResults);

            //--assert
            result.WinnerType.ShouldBe(WinnerTypes.PlayerWin);
        }
    }
}
