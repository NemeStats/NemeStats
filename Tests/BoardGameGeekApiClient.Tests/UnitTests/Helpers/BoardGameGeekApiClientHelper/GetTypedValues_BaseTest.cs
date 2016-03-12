using System.Collections.Generic;
using System.Xml.Linq;

namespace BoardGameGeekApiClient.Tests.UnitTests.Helpers.BoardGameGeekApiClientHelper
{
    public abstract class GetTypedValues_BaseTest : BoardGameGeekApiClientHelper_Tests
    {
        public List<string> Artists { get; set; } = new List<string>
        {
            "<link type='boardgameartist' id='16531' value='Kevin Childress'/>",
            "<link type='boardgameartist' id='6651' value='Corey Konieczka'/>",
            "<link type='boardgameartist' id='12018' value='Andrew Navaro'/>",
            "<link type='boardgameartist' id='11989' value='Brian Schomburg'/>",
            "<link type='boardgameartist' id='12436' value='WiL Springer'/>"
        };

        public List<string> Mechanincs { get; set; } = new List<string>
        {
            "<link type='boardgamemechanic' id='2046' value='Area Movement'/>" ,
            "<link type='boardgamemechanic' id='2023' value='Co-operative Play'/>" ,
            "<link type='boardgamemechanic' id='2072' value='Dice Rolling'/>" ,
            "<link type='boardgamemechanic' id='2040' value='Hand Management'/>" ,
            "<link type='boardgamemechanic' id='2019' value='Partnerships'/>" ,
            "<link type='boardgamemechanic' id='2028' value='Role Playing'/>" ,
            "<link type='boardgamemechanic' id='2015' value='Variable Player Powers'/>" ,
        };

        public List<string> Publishers { get; set; } = new List<string>
        {

            "<link type='boardgamepublisher' id='17' value='Fantasy Flight Games'/>" ,
            "<link type='boardgamepublisher' id='2973' value='Edge Entertainment'/>" ,
            "<link type='boardgamepublisher' id='253' value='Editrice Giochi'/>" ,
            "<link type='boardgamepublisher' id='4617' value='Galakta'/>" ,
            "<link type='boardgamepublisher' id='12540' value='Game Harbor'/>" ,
            "<link type='boardgamepublisher' id='5530' value='Giochi Uniti'/>" ,
            "<link type='boardgamepublisher' id='264' value='Heidelberger Spieleverlag'/>" ,
        };

        public List<string> Categories { get; set; } = new List<string>
        {

            "<link type='boardgamecategory' id='1023' value='Bluffing'/>" ,
            "<link type='boardgamecategory' id='1039' value='Deduction'/>" ,
            "<link type='boardgamecategory' id='1064' value='Movies / TV / Radio theme'/>" ,
            "<link type='boardgamecategory' id='1026' value='Negotiation'/>" ,
            "<link type='boardgamecategory' id='1001' value='Political'/>" ,
            "<link type='boardgamecategory' id='1016' value='Science Fiction'/>" ,
            "<link type='boardgamecategory' id='1113' value='Space Exploration'/>" ,
            "<link type='boardgamecategory' id='1081' value='Spies/Secret Agents'/>" ,
        };

        public List<string> Designers { get; set; } = new List<string>
        {
            "<link type='boardgamedesigner' id='6651' value='Corey Konieczka'/>"
        };

        public List<string> Expansions { get; set; } = new List<string>
        {
                                "<link type='boardgameexpansion' id='141648' value='Battlestar Galactica: Daybreak Expansion'/>" ,
                    "<link type='boardgameexpansion' id='85905' value='Battlestar Galactica: Exodus Expansion'/>" ,
                    "<link type='boardgameexpansion' id='105136' value='Battlestar Galactica: Official Variant Rules'/>" ,
                    "<link type='boardgameexpansion' id='43539' value='Battlestar Galactica: Pegasus Expansion'/>" ,
        };

        protected override void SetXElementToTest()
        {
            XElementToTest =
                XDocument.Parse(
                    @"<items termsofuse='http://boardgamegeek.com/xmlapi/termsofuse'>" +
                    "<item type='boardgame' id='37111'>" +
                    string.Join("", Categories) +
                    string.Join("", Mechanincs) +
                    "<link type='boardgamefamily' id='4685' value='Battlestar Galactica'/>" +
                    string.Join("", Expansions) +
                    string.Join("", Designers) +
                    string.Join("", Artists) +
                    string.Join("", Publishers) +
                    "</item></items>").Root.Element("item");
        }
    }
}