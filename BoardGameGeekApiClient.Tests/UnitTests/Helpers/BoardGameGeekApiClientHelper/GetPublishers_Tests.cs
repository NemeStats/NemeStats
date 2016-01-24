using System.Collections.Generic;
using System.Linq;
using BoardGameGeekApiClient.Helpers;
using NUnit.Framework;

namespace BoardGameGeekApiClient.Tests.UnitTests.Helpers.BoardGameGeekApiClientHelper
{
    public class GetPublishers_Tests : GetTypedValues_BaseTest
    {
        public List<string> Result { get; set; }

        public class When_Has_Publishers : GetPublishers_Tests
        {
            [SetUp]
            public override void SetUp()
            {
                base.SetUp();
                Result = XElementToTest.GetPublishers();
            }

            [Test]
            public void Then_Return_Expected_Data()
            {
                Assert.IsNotEmpty(Result);
                Assert.AreEqual(Publishers.Count, Result.Count);
                Assert.AreEqual("Fantasy Flight Games", Result.First());
            }
        }

        public class When_Has_No_Publishers : GetPublishers_Tests
        {
            [SetUp]
            public override void SetUp()
            {
                Publishers = new List<string>();
                base.SetUp();
                Result = XElementToTest.GetPublishers();
            }

            [Test]
            public void Then_Return_Empty_List()
            {
                Assert.IsEmpty(Result);
            }
        }
    }
}