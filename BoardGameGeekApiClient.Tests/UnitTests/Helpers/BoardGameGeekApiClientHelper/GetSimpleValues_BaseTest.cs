using System.Xml.Linq;

namespace BoardGameGeekApiClient.Tests.UnitTests.BoardGameGeekApiClientHelper
{
    public class GetSimpleValues_BaseTest : BoardGameGeekApiClientHelper_Tests
    {
        public string ElementValue { get; set; } = "Betting/Wagering";

        public string Attribute { get; set; } = "objectid";

        public string AttributeValue { get; set; } = "2014";

        protected override void SetXElementToTest()
        {
            XElementToTest = XDocument.Parse(string.Format("<boardgamemechanic {0}='{1}'>{2}</boardgamemechanic>", Attribute, AttributeValue, ElementValue)).Root;
        }

    }
}