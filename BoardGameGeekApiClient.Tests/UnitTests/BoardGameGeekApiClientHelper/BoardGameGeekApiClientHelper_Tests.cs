using System.Xml.Linq;
using NUnit.Framework;

namespace BoardGameGeekApiClient.Tests.UnitTests.BoardGameGeekApiClientHelper
{
    [TestFixture]
    public class BoardGameGeekApiClientHelper_Tests
    {
        public XElement XElementToTest { get; set; }
        public string ElementValue { get; set; }
        public string Attribute { get; set; }
        public string AttributeValue { get; set; }

        [SetUp]
        public virtual void SetUp()
        {
            ElementValue = "Betting/Wagering";
            Attribute = "objectid";
            AttributeValue = "2014";
            XElementToTest = XDocument.Parse(string.Format("<boardgamemechanic {0}='{1}'>{2}</boardgamemechanic>", Attribute, AttributeValue, ElementValue)).Root;
        }

    }
}
