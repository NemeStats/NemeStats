#region LICENSE
// NemeStats is a free website for tracking the results of board games.
//     Copyright (C) 2015 Jacob Gordon
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>
#endregion
using NUnit.Framework;
using System.Linq;
using UI.Controllers.Helpers;

namespace UI.Tests.UnitTests.ControllerTests.HelperTests.ShowingXResultsMessageBuilderTests
{
    [TestFixture]
    public class BuildMessageTests
    {
        protected IShowingXResultsMessageBuilder messageBuilder;

        [SetUp]
        public void SetUp()
        {
            messageBuilder = new ShowingXResultsMessageBuilder();
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
            string expectedMessage = string.Format(ShowingXResultsMessageBuilder.RECENT_GAMES_MESSAGE_FORMAT, maxResults);

            string message = messageBuilder.BuildMessage(maxResults, actualResults);

            Assert.AreEqual(expectedMessage, message);
        }
    }
}
