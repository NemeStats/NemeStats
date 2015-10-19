using System.Xml.Linq;
using NUnit.Framework;

namespace BoardGameGeekApiClient.Tests.UnitTests.BoardGameGeekApiClientHelper
{
    /// <summary>
    /// Based on real result for Battle Star Galactica
    /// http://www.boardgamegeek.com/xmlapi2/thing?id=37111&stats=1
    /// </summary>
    [TestFixture]
    public abstract class BoardGameGeekApiClientHelper_Tests
    {
        public XElement XElementToTest { get; set; }

       
        [SetUp]
        public virtual void SetUp()
        {
            SetXElementToTest();
        }

        protected abstract void SetXElementToTest();

    }
}
