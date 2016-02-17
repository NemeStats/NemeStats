using BoardGameGeekApiClient.Helpers;
using NUnit.Framework;

namespace BoardGameGeekApiClient.Tests.UnitTests.Helpers.BoardGameGeekApiClientHelper
{
    public class GetIntValue_Tests : GetSimpleValues_BaseTest
    {
        public int DefaultValue { get; set; }
        public int? Result { get; set; }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            DefaultValue = 1;

        }

        public class When_XElement_Is_Null : GetIntValue_Tests
        {
            [SetUp]
            public override void SetUp()
            {
                base.SetUp();
                XElementToTest = null;
                Result = XElementToTest.GetIntValue(Attribute, DefaultValue);
            }

            [Test]
            public void Then_Returns_DefaultValue()
            {
                Assert.AreEqual(Result, DefaultValue);

            }
        }

        public class When_Attribute_Is_Null_And_Element_Value_Is_Integer : GetIntValue_Tests
        {
            int integerElementValue = 100;

            [SetUp]
            public override void SetUp()
            {
                
                ElementValue = integerElementValue.ToString();
                base.SetUp();
                Attribute = null;
                Result = XElementToTest.GetIntValue(Attribute, DefaultValue);
            }

            [Test]
            public void Then_Returns_Element_Value()
            {
                Assert.AreEqual(Result, integerElementValue);
            }
        }

        public class When_Attribute_Is_Null_And_Element_Value_Not_Is_Integer : GetIntValue_Tests
        {

            [SetUp]
            public override void SetUp()
            {

                ElementValue = "not_a_integer";
                base.SetUp();
                Attribute = null;
                Result = XElementToTest.GetIntValue(Attribute, DefaultValue);
            }

            [Test]
            public void Then_Returns_Default_Value()
            {
                Assert.AreEqual(Result, DefaultValue);
            }
        }

        public class When_Attribute_Not_Exists : GetIntValue_Tests
        {
            [SetUp]
            public override void SetUp()
            {
                base.SetUp();
                Attribute = "not_exists";
                Result = XElementToTest.GetIntValue(Attribute, DefaultValue);
            }

            [Test]
            public void Then_Returns_DefaultValue()
            {
                Assert.AreEqual(Result, DefaultValue);
            }
        }

        public class When_Attribute_Exists_And_Is_Integer : GetIntValue_Tests
        {
            int integerAttributeValue = 100;
            [SetUp]
            public override void SetUp()
            {
                AttributeValue = integerAttributeValue.ToString();
                base.SetUp();
                Result = XElementToTest.GetIntValue(Attribute, DefaultValue);
            }

            [Test]
            public void Then_Returns_Attribute_Value()
            {
                Assert.AreEqual(Result, int.Parse(AttributeValue));
            }
        }

        public class When_Attribute_Exists_And_Not_Is_Integer : GetIntValue_Tests
        {
            [SetUp]
            public override void SetUp()
            {
                AttributeValue = "not_integer";
                base.SetUp();
                Result = XElementToTest.GetIntValue(Attribute, DefaultValue);
            }

            [Test]
            public void Then_Returns_DefaultValue()
            {
                Assert.AreEqual(Result, DefaultValue);
            }
        }

    }
}