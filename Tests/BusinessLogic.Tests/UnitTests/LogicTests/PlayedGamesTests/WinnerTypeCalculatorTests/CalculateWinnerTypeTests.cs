using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Logic.PlayedGames;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using BusinessLogic.Models.PlayedGames;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Shouldly;
using StructureMap.AutoMocking;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayedGamesTests.WinnerTypeCalculatorTests
{
    [TestFixture]
    public class CalculateWinnerTypeTests
    {
        private readonly WinnerTypeCalculator _winnerTypeCalculator = new WinnerTypeCalculator();

        [Test]
        public void It_Sets_The_WinnerType_To_Team_Win_If_All_Players_Won()
        {
            //--arrange
            var gameRanks = new List<int> {1, 1, 1};

            //--act
            var result = _winnerTypeCalculator.CalculateWinnerType(gameRanks);

            //--assert
            result.ShouldBe(WinnerTypes.TeamWin);
        }

        [Test]
        public void It_Sets_The_WinnerType_To_Team_Loss_If_All_Players_Lost()
        {
            //--arrange
            var gameRanks = new List<int> { 2, 2, 2 };

            //--act
            var result = _winnerTypeCalculator.CalculateWinnerType(gameRanks);

            //--assert
            result.ShouldBe(WinnerTypes.TeamLoss);
        }

        [Test]
        public void It_Sets_The_WinnerType_To_Player_Win_If_There_Was_At_Least_One_Winner_And_One_Loser()
        {
            //--arrange
            var gameRanks = new List<int> { 1, 1, 2 };

            //--act
            var result = _winnerTypeCalculator.CalculateWinnerType(gameRanks);

            //--assert
            result.ShouldBe(WinnerTypes.PlayerWin);
        }
    }
}
