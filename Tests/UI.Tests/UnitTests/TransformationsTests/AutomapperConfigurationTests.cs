using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
