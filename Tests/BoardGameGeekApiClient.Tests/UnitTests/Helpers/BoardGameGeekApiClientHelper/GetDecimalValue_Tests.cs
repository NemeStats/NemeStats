using BoardGameGeekApiClient.Helpers;
using NUnit.Framework;

namespace BoardGameGeekApiClient.Tests.UnitTests.Helpers.BoardGameGeekApiClientHelper
{
    public class GetDecimalValue_Tests : GetSimpleValues_BaseTest
    {
        public decimal DefaultValue { get; set; }
        public decimal? Result { get; set; }

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            DefaultValue = 1;

        }

        public class When_XElement_Is_Null : GetDecimalValue_Tests
        {
            [SetUp]
            public override void SetUp()
            {
                base.SetUp();
                XElementToTest = null;
                Result = XElementToTest.GetDecimalValue(Attribute, DefaultValue);
            }

            [Test]
            public void Then_Returns_DefaultValue()
            {
                Assert.AreEqual(Result, DefaultValue);

            }
        }

        public class When_Attribute_Is_Null_And_Element_Value_Is_Decimal : GetDecimalValue_Tests
        {
            decimal decimalElementValue = (decimal) 100.1;

            [SetUp]
            public override void SetUp()
            {

                ElementValue = decimalElementValue.ToString();
                base.SetUp();
                Attribute = null;
                Result = XElementToTest.GetDecimalValue(Attribute, DefaultValue);
            }

            [Test]
            public void Then_Returns_Element_Value()
            {
                Assert.AreEqual(Result, decimalElementValue);
            }
        }

        public class When_Attribute_Is_Null_And_Element_Value_Not_Is_Decimal : GetDecimalValue_Tests
        {

            [SetUp]
            public override void SetUp()
            {

                ElementValue = "not_a_decimal";
                base.SetUp();
                Attribute = null;
                Result = XElementToTest.GetDecimalValue(Attribute, DefaultValue);
            }

            [Test]
            public void Then_Returns_Default_Value()
            {
                Assert.AreEqual(Result, DefaultValue);
            }
        }

        public class When_Attribute_Not_Exists : GetDecimalValue_Tests
        {
            [SetUp]
            public override void SetUp()
            {
                base.SetUp();
                Attribute = "not_exists";
                Result = XElementToTest.GetDecimalValue(Attribute, DefaultValue);
            }

            [Test]
            public void Then_Returns_DefaultValue()
            {
                Assert.AreEqual(Result, DefaultValue);
            }
        }

        public class When_Attribute_Exists_And_Is_Decimal : GetDecimalValue_Tests
        {
            decimal integerAttributeValue = (decimal) 100.1;
            [SetUp]
            public override void SetUp()
            {
                AttributeValue = integerAttributeValue.ToString();
                base.SetUp();
                Result = XElementToTest.GetDecimalValue(Attribute, DefaultValue);
            }

            [Test]
            public void Then_Returns_Attribute_Value()
            {
                Assert.AreEqual(Result, decimal.Parse(AttributeValue));
            }
        }

        public class When_Attribute_Exists_And_Not_Is_Decimal : GetDecimalValue_Tests
        {
            [SetUp]
            public override void SetUp()
            {
                AttributeValue = "not_number";
                base.SetUp();
                Result = XElementToTest.GetDecimalValue(Attribute, DefaultValue);
            }

            [Test]
            public void Then_Returns_DefaultValue()
            {
                Assert.AreEqual(Result, DefaultValue);
            }
        }

    }
}