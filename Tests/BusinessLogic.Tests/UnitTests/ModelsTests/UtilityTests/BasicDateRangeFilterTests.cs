using BusinessLogic.Models.Utility;
using NUnit.Framework;
using System;

namespace BusinessLogic.Tests.UnitTests.ModelsTests.UtilityTests
{
    [TestFixture]
    public class BasicDateRangeFilterTests
    {
        private BasicDateRangeFilter filter;

        [SetUp]
        public void SetUp()
        {
            filter = new BasicDateRangeFilter();
        }

        [Test]
        public void ItDefaultsTheFromDateToJanuaryFirstTwoThousandAndTen()
        {
            Assert.That(filter.FromDate, Is.EqualTo(new DateTime(2010, 1, 1)));
        }

        [Test]
        public void ItDefaultsTheFromDateToTheLastMillisecondUtcToday()
        {
            Assert.That(filter.ToDate, Is.EqualTo(DateTime.UtcNow.Date.AddDays(1).AddMilliseconds(-1)));
        }

        [Test]
        public void TheIso8601FromDatePropertyReturnsAnExistingFromDateInYYYYMMDD()
        {
            Assert.That(filter.Iso8601FromDate, Is.EqualTo("2010-01-01"));
        }

        [Test]
        public void TheIso8601ToDatePropertyReturnsAnExistingToDateInYYYYMMDD()
        {
            Assert.That(filter.Iso8601ToDate, Is.EqualTo(DateTime.UtcNow.ToString("yyyy-MM-dd")));
        }

        [Test]
        public void TheIso8601FromDateSetterAlsoChangesTheFromDate()
        {
            filter.Iso8601FromDate = "2014-07-17";

            Assert.That(filter.FromDate, Is.EqualTo(new DateTime(2014, 7, 17)));
        }

        [Test]
        public void TheIso8601ToDateSetterAlsoChangesTheToDate()
        {
            filter.Iso8601ToDate = "2017-06-15";

            Assert.That(filter.ToDate, Is.EqualTo(new DateTime(2017, 6, 15)));
        }

        [Test]
        public void ItThrowsAFormatExceptionIfTheFromDateIsInvalid()
        {
            var invalidDateString = "some invalid date";
            var exception = Assert.Throws<FormatException>(() => filter.Iso8601FromDate = invalidDateString);

            Assert.That(exception.Message, Is.EqualTo("'some invalid date' is not a valid YYYY-MM-DD date."));
        }

        [Test]
        public void ItThrowsAFormatExceptionIfTheToDateIsInvalid()
        {
            var invalidDateString = "some invalid date";
            var exception = Assert.Throws<FormatException>(() => filter.Iso8601ToDate = invalidDateString);

            Assert.That(exception.Message, Is.EqualTo("'some invalid date' is not a valid YYYY-MM-DD date."));
        }
    }
}
