using System;
using BusinessLogic.Exceptions;
using NUnit.Framework;
using Shouldly;

namespace BusinessLogic.Tests.UnitTests.LogicTests.PlayedGamesTests.PlayedGameSaverTests
{
    [TestFixture()]
    public class ValidateDatePlayedTests : PlayedGameSaverTestBase
    {
        [Test]
        public void It_Doesnt_Throw_An_Exception_If_The_Date_Played_Is_Less_Than_Two_Days_In_The_Future()
        {
            //--arrange
            var oneDayInTheFuture = DateTime.UtcNow.Date.AddDays(1);

            //--act
            AutoMocker.ClassUnderTest.ValidateDatePlayed(oneDayInTheFuture);
        }

        [Test]
        public void It_Throws_An_InvalidPlayedGameDateException_If_The_Games_Is_Recorded_More_Than_Two_Days_In_The_Future()
        {
            //--arrange
            var twoDaysInTheFuture = DateTime.UtcNow.Date.AddDays(2);
            var expectedException = new InvalidPlayedGameDateException(twoDaysInTheFuture);

            //--act
            var actualException = Assert.Throws<InvalidPlayedGameDateException>(() => AutoMocker.ClassUnderTest.ValidateDatePlayed(twoDaysInTheFuture));

            //--assert
            actualException.Message.ShouldBe(expectedException.Message);
        }
    }
}
