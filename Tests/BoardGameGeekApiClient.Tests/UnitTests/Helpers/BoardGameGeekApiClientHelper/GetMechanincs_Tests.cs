using System.Collections.Generic;
using System.Linq;
using BoardGameGeekApiClient.Helpers;
using NUnit.Framework;

namespace BoardGameGeekApiClient.Tests.UnitTests.Helpers.BoardGameGeekApiClientHelper
{
    public class GetMechanincs_Tests : GetTypedValues_BaseTest
    {
        public List<string> Result { get; set; }

        public class When_Has_Mechanincs : GetMechanincs_Tests
        {
            [SetUp]
            public override void SetUp()
            {
                base.SetUp();
                Result = XElementToTest.GetMechanics();
            }

            [Test]
            public void Then_Return_Expected_Data()
            {
                Assert.IsNotEmpty(Result);
                Assert.AreEqual(Mechanincs.Count, Result.Count);
                Assert.AreEqual("Area Movement", Result.First());
            }
        }

        public class When_Has_No_Mechanincs : GetMechanincs_Tests
        {
            [SetUp]
            public override void SetUp()
            {
                Mechanincs = new List<string>();
                base.SetUp();
                Result = XElementToTest.GetMechanics();
            }

            [Test]
            public void Then_Return_Empty_List()
            {
                Assert.IsEmpty(Result);
            }
        }
    }
}