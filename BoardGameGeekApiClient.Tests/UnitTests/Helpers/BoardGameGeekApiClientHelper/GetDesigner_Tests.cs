using System.Collections.Generic;
using System.Linq;
using BoardGameGeekApiClient.Helpers;
using NUnit.Framework;

namespace BoardGameGeekApiClient.Tests.UnitTests.BoardGameGeekApiClientHelper
{
    public class GetDesigner_Tests : GetTypedValues_BaseTest
    {
        public List<string> Result { get; set; }

        public class When_Has_Designers : GetDesigner_Tests
        {
            [SetUp]
            public override void SetUp()
            {
                base.SetUp();
                Result = XElementToTest.GetDesigners();
            }

            [Test]
            public void Then_Return_Expected_Data()
            {
                Assert.IsNotEmpty(Result);
                Assert.AreEqual(Designers.Count, Result.Count);
                Assert.AreEqual("Corey Konieczka", Result.First());
            }
        }

        public class When_Has_No_Designers : GetDesigner_Tests
        {
            [SetUp]
            public override void SetUp()
            {
                Designers = new List<string>();
                base.SetUp();
                Result = XElementToTest.GetDesigners();
            }

            [Test]
            public void Then_Return_Empty_List()
            {
                Assert.IsEmpty(Result);
            }
        }
    }
}