using AutoMapper;
using NUnit.Framework;

namespace UI.Tests.UnitTests.TransformationsTests
{
    [TestFixture]
    public class AutomapperConfigurationTests
    {
        [Test]
        public void TestConfiguration()
        {
            Mapper.AssertConfigurationIsValid();
        }
    }
}
