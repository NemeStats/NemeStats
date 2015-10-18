using BoardGameGeekApiClient.Helpers;
using NUnit.Framework;

namespace BoardGameGeekApiClient.Tests.UnitTests.BoardGameGeekApiClientHelper
{
    public class GetStringValue_Tests : GetSimpleValues_BaseTest
    {
        public string DefaultValue { get; set; }
        public string Result { get; set; }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            DefaultValue = "default value";

        }

        public class When_XElement_Is_Null : GetStringValue_Tests
        {
            [SetUp]
            public override void SetUp()
            {
                base.SetUp();
                XElementToTest = null;
                Result = XElementToTest.GetStringValue(Attribute, DefaultValue);
            }

            [Test]
            public void Then_Returns_DefaultValue()
            {
                Assert.AreEqual(Result, DefaultValue);

            }
        }

        public class When_Attribute_Is_Null : GetStringValue_Tests
        {
            [SetUp]
            public override void SetUp()
            {
                base.SetUp();
                Attribute = null;
                Result = XElementToTest.GetStringValue(Attribute, DefaultValue);
            }

            [Test]
            public void Then_Returns_Element_Value()
            {
                Assert.AreEqual(Result, ElementValue);
            }
        }

        public class When_Attribute_Not_Exists : GetStringValue_Tests
        {
            [SetUp]
            public override void SetUp()
            {
                base.SetUp();
                Attribute = "not_exists";
                Result = XElementToTest.GetStringValue(Attribute, DefaultValue);
            }

            [Test]
            public void Then_Returns_DefaultValue()
            {
                Assert.AreEqual(Result, DefaultValue);
            }
        }

        public class When_Attribute_Exists : GetStringValue_Tests
        {
            [SetUp]
            public override void SetUp()
            {
                base.SetUp();
                Result = XElementToTest.GetStringValue(Attribute, DefaultValue);
            }

            [Test]
            public void Then_Returns_Attribute_Value()
            {
                Assert.AreEqual(Result, AttributeValue);
            }
        }

    }
}