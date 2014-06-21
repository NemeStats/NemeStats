using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml.Linq;
using UI.HtmlHelpers;
using UI.Models.PlayedGame;

namespace UI.Tests.UnitTests.HtmlHelperTests.MyHelpersTests
{
    [TestFixture]
    public class GameResultsTests
    {
        [Test]
        public void ItRequiresPlayerGameResultDetails()
        {
            HtmlHelper helper = new HtmlHelper(new ViewContext(), new ViewPage());
            
            var exception = Assert.Throws<ArgumentNullException>(() =>
                helper.GameResults(null));

            Assert.AreEqual("playerGameResultDetails", exception.ParamName);
        }

        [Test]
        public void ItRendersTheGameRankInASpanWithTheGameRankClass()
        {
            HtmlHelper helper = new HtmlHelper(new ViewContext(), new ViewPage());
            PlayerGameResultDetails playerGameResultDetails = new PlayerGameResultDetails()
            {
                GameRank = 1151
            };

            XElement result = helper.GameResults(playerGameResultDetails).ToXElement();

            Assert.AreEqual("span", result.Name.ToString());
            Assert.True(result.FirstAttribute.ToString().Contains("class=\"" + PlayedGameHelper.CSS_CLASS_GAME_RANK));
        }

        private void TestRenderingForGivenRank(int gameRank, string expectedCSSClass, string expectedRankText)
        {
            HtmlHelper helper = new HtmlHelper(new ViewContext(), new ViewPage());
            PlayerGameResultDetails playerGameResultDetails = new PlayerGameResultDetails()
            {
                GameRank = gameRank
            };

            XElement result = helper.GameResults(playerGameResultDetails).ToXElement();

            string firstAttribute = result.FirstAttribute.ToString();
            Assert.True(firstAttribute.Contains("class=\"") && firstAttribute.Contains(expectedCSSClass));
            string firstNodeText = result.FirstNode.ToString();
            Assert.True(firstNodeText.StartsWith(expectedRankText));
        }

        [Test]
        public void ItRendersFirstPlace()
        {
            TestRenderingForGivenRank(1, PlayedGameHelper.CSS_CLASS_FIRST_PLACE, PlayedGameHelper.PLACE_FIRST);
        }

        [Test]
        public void ItRendersSecondPlace()
        {
            TestRenderingForGivenRank(2, PlayedGameHelper.CSS_CLASS_SECOND_PLACE, PlayedGameHelper.PLACE_SECOND);
        }

        [Test]
        public void ItRendersThirdPlace()
        {
            TestRenderingForGivenRank(3, PlayedGameHelper.CSS_CLASS_THIRD_PLACE, PlayedGameHelper.PLACE_THIRD);
        }

        [Test]
        public void ItRendersFourthPlace()
        {
            TestRenderingForGivenRank(4, PlayedGameHelper.CSS_CLASS_FOURTH_PLACE, PlayedGameHelper.PLACE_FOURTH);
        }

        [Test]
        public void ItRendersTheBigLoser()
        {
            TestRenderingForGivenRank(5, PlayedGameHelper.CSS_CLASS_LOSER_PLACE, PlayedGameHelper.PLACE_BIG_LOSER);
        }

        [Test]
        public void ItRendersGordonPoints()
        {
            HtmlHelper helper = new HtmlHelper(new ViewContext(), new ViewPage());
            PlayerGameResultDetails playerGameResultDetails = new PlayerGameResultDetails()
            {
                GordonPoints = 9
            };

            XElement result = helper.GameResults(playerGameResultDetails).ToXElement();

            string firstNodeText = result.FirstNode.ToString();
            string gordonPointsComponent = string.Format(PlayedGameHelper.HTML_GORDON_POINTS_TEMPLATE, playerGameResultDetails.GordonPoints);
            Assert.True(firstNodeText.EndsWith(gordonPointsComponent));
        }
    }
}
