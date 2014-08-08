using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Controllers.Helpers;

namespace UI.Tests.UnitTests.ControllerTests.HelperTests.ShowingXResultsMessageBuilderTests
{
    [TestFixture]
    public class BuildMessageTests
    {
        protected ShowingXResultsMessageBuilder messageBuilder;

        [SetUp]
        public void SetUp()
        {
            messageBuilder = new ShowingXResultsMessageBuilderImpl();
        }

        [Test]
        public void ItReturnsBlankIfTheActualNumberOfResultsIsLessThanTheMax()
        {
            string message = messageBuilder.BuildMessage(2, 1);

            Assert.AreEqual(string.Empty, message);
        }

        [Test]
        public void ItReturnsAMessageStatingThatItIsOnlyShowingTheMaxNumberOfResultsIfTheActualNumberOfResultsEqualsOrExceedsTheMax()
        {
            int maxResults = 1;
            int actualResults = 1;
            string expectedMessage = string.Format(ShowingXResultsMessageBuilderImpl.RECENT_GAMES_MESSAGE_FORMAT, maxResults);

            string message = messageBuilder.BuildMessage(maxResults, actualResults);

            Assert.AreEqual(expectedMessage, message);
        }
    }
}
