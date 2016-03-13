using System.Xml.Linq;
using BoardGameGeekApiClient.Helpers;
using NUnit.Framework;

namespace BoardGameGeekApiClient.Tests.UnitTests.Helpers.BoardGameGeekApiClientHelper
{
    public abstract class GetBoardGameNameTests : BoardGameGeekApiClientHelper_Tests
    {
        public string Result { get; set; }

        protected abstract override void SetXElementToTest();

        class When_Item_Has_Name : GetBoardGameNameTests
        {
            private string name = "Battlestar Galactica";

            protected override void SetXElementToTest()
            {
                XElementToTest =
                    XDocument.Parse(
                        @"<items termsofuse='http://boardgamegeek.com/xmlapi/termsofuse'>" +
                        "<item type='boardgame' id='37111'><thumbnail>//cf.geekdo-images.com/images/pic354500_t.jpg</thumbnail>" +
                        "<image>//cf.geekdo-images.com/images/pic354500.jpg</image>" +
                        string.Format("<name type='primary' sortindex='1' value = '{0}' />", name) +
                        "<name type='alternate' sortindex='1' value='Battlestar Galactica: Das Brettspiel' />" +
                        "<name type='alternate' sortindex='1' value='Battlestar Galactica: El juego de tablero' />" +
                        "<name type='alternate' sortindex='1' value='Battlestar Galactica: Gra planszowa' />" +
                        "<name type='alternate' sortindex='1' value='Battlestar Galactica: il gioco da tavolo' />" +
                        "<name type='alternate' sortindex='1' value='Battlestar Galactica: Le Jeu de Plateau' />" +
                        "<name type='alternate' sortindex='1' value='太空堡垒 卡拉狄加' />" +
                        "<name type='alternate' sortindex='1' value='太空堡垒卡拉狄加' /> " +
                        "</item></items>").Root;
            }

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();
                Result = XElementToTest.Element("item").GetBoardGameName();
            }

            [Test]
            public void Then_Name_Is_Returned()
            {
                Assert.AreEqual(name, Result);
            }
        }

        class When_Item_Has_No_Name : GetBoardGameNameTests
        {
            protected override void SetXElementToTest()
            {
                XElementToTest =
                    XDocument.Parse(
                        @"<items termsofuse='http://boardgamegeek.com/xmlapi/termsofuse'>" +
                        "<item type='boardgame' id='37111'><thumbnail>//cf.geekdo-images.com/images/pic354500_t.jpg</thumbnail>" +
                        "<image>//cf.geekdo-images.com/images/pic354500.jpg</image>" +
                        "</item></items>").Root;
            }

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();
                Result = XElementToTest.Element("item").GetBoardGameName();
            }

            [Test]
            public void Then_Return_Null()
            {
                Assert.IsNull(Result);
            }
        }

    }
}