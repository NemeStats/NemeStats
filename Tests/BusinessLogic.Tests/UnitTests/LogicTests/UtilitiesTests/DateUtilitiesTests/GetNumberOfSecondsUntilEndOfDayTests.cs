using System;
using BusinessLogic.Logic.Utilities;
using NUnit.Framework;

namespace BusinessLogic.Tests.UnitTests.LogicTests.UtilitiesTests.DateUtilitiesTests
{
    [TestFixture]
    public class GetNumberOfSecondsUntilEndOfDayTests
    {
        [Test]
        public void ItExpiresAtUTCMidnight()
        {
            //--arrange
            var timeSpanUntilUTCMidnight = DateTime.UtcNow.Date.AddDays(1) - DateTime.UtcNow;
            int numberOfSecondsUntilMidnight = (int)timeSpanUntilUTCMidnight.TotalSeconds;

            //--act
            var results = new DateUtilities().GetNumberOfSecondsUntilEndOfDay();

            //--assert
            var differenceBetweenExpectedAndActual = numberOfSecondsUntilMidnight - results;
            Assert.That(differenceBetweenExpectedAndActual, Is.LessThan(10));
        }

    }
}
