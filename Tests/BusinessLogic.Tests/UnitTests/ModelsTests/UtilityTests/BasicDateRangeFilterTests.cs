using BusinessLogic.Models.Utility;
using NUnit.Framework;
using System;

namespace BusinessLogic.Tests.UnitTests.ModelsTests.UtilityTests
{
    [TestFixture]
    public class BasicDateRangeFilterTests
    {
        protected BasicDateRangeFilter filter;

        [SetUp]
        public void SetUp()
        {
            filter = new BasicDateRangeFilter();
        }

        public class WhenCallingTheConstructor : BasicDateRangeFilterTests
        {
            [Test]
            public void ItDefaultsTheFromDateToJanuaryFirstTwoThousandAndFourteen()
            {
                Assert.That(filter.FromDate, Is.EqualTo(new DateTime(2014, 1, 1)));
            }

            [Test]
            public void ItDefaultsTheFromDateToTheLastMillisecondUtcToday()
            {
                Assert.That(filter.ToDate, Is.EqualTo(DateTime.UtcNow.Date.AddDays(1).AddMilliseconds(-1)));
            }
        }

        public class WhenUsingTheFromAndToDateDateProperties : BasicDateRangeFilterTests
        {
            [Test]
            public void TheIso8601FromDatePropertyReturnsAnExistingFromDateInYYYYMMDD()
            {
                Assert.That(filter.Iso8601FromDate, Is.EqualTo("2014-01-01"));
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
       
        public class WhenCallingIsValid : BasicDateRangeFilterTests
        {
            [Test]
            public void ItReturnsTrueIfThereAreNoValidationIssues()
            {
                string errorMessage;
                bool isValid = filter.IsValid(out errorMessage);

                Assert.That(isValid, Is.EqualTo(true));
                Assert.That(errorMessage, Is.Null);
            }

            [Test]
            public void ItReturnsFalseAndGivesAnErrorMessageIfTheFromDateIsAfterTheToDate()
            {
                string errorMessage;
                DateTime toDate = DateTime.Now.AddDays(-5);
                filter.ToDate = toDate;
                filter.FromDate = filter.ToDate.AddDays(1);

                bool isValid = filter.IsValid(out errorMessage);

                Assert.That(isValid, Is.EqualTo(false));
                Assert.That(errorMessage, Is.EqualTo("The 'From Date' cannot be greater than the 'To Date'."));
            }

            [Test]
            public void ItReturnsFalseAndGivesAnErrorMessageIfTheFromDateIsInTheFuture()
            {
                string errorMessage;
                filter.FromDate = DateTime.UtcNow.AddDays(2);

                bool isValid = filter.IsValid(out errorMessage);

                Assert.That(isValid, Is.EqualTo(false));
                Assert.That(errorMessage, Is.EqualTo("The 'From Date' cannot be in the future."));
            }

            [Test]
            public void ItReturnsFalseAndGivesAnErrorMessageIfTheToDateIsInTheFuture()
            {
                string errorMessage;
                filter.ToDate = DateTime.UtcNow.AddDays(2);

                bool isValid = filter.IsValid(out errorMessage);

                Assert.That(isValid, Is.EqualTo(false));
                Assert.That(errorMessage, Is.EqualTo("The 'Ending Date' cannot be in the future."));
            }
        }
    }
}
