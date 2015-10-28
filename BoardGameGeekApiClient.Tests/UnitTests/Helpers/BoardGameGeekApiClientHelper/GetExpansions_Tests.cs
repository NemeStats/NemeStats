using System.Collections.Generic;
using System.Linq;
using BoardGameGeekApiClient.Helpers;
using BoardGameGeekApiClient.Models;
using NUnit.Framework;

namespace BoardGameGeekApiClient.Tests.UnitTests.BoardGameGeekApiClientHelper
{
    public class GetExpansions_Tests : GetTypedValues_BaseTest
    {
        public List<BoardGameLink> Result { get; set; }

        public class When_Has_Expansions : GetExpansions_Tests
        {
            [SetUp]
            public override void SetUp()
            {
                base.SetUp();
                Result = XElementToTest.GetExpansionsLinks();
            }

            [Test]
            public void Then_Return_Expected_Data()
            {
                Assert.IsNotEmpty(Result);
                Assert.AreEqual(Expansions.Count, Result.Count);
                Assert.AreEqual("Battlestar Galactica: Daybreak Expansion", Result.First().Name);
                Assert.AreEqual(141648, Result.First().Id);
            }
        }

        public class When_Has_No_Expansions : GetExpansions_Tests
        {
            [SetUp]
            public override void SetUp()
            {
                Expansions = new List<string>();
                base.SetUp();
                Result = XElementToTest.GetExpansionsLinks();
            }

            [Test]
            public void Then_Return_Empty_List()
            {
                Assert.IsEmpty(Result);
            }
        }
    }
}