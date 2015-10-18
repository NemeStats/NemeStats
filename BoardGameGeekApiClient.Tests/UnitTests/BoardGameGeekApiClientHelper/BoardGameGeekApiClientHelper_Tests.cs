using System.Xml.Linq;
using NUnit.Framework;

namespace BoardGameGeekApiClient.Tests.UnitTests.BoardGameGeekApiClientHelper
{
    [TestFixture]
    public class BoardGameGeekApiClientHelper_Tests
    {
        public XElement XElementToTest { get; set; }

        public string ElementValue { get; set; } = "Betting/Wagering";

        public string Attribute { get; set; } = "objectid";

        public string AttributeValue { get; set; } = "2014";

        [SetUp]
        public virtual void SetUp()
        {
            XElementToTest = XDocument.Parse(string.Format("<boardgamemechanic {0}='{1}'>{2}</boardgamemechanic>", Attribute, AttributeValue, ElementValue)).Root;
        }

    }
}
