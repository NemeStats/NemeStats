using System.Collections.Generic;
using System.Linq;
using BoardGameGeekApiClient.Helpers;
using NUnit.Framework;

namespace BoardGameGeekApiClient.Tests.UnitTests.Helpers.BoardGameGeekApiClientHelper
{
    public class GetArtist_Tests : GetTypedValues_BaseTest
    {
        public List<string> Result { get; set; }

        public class When_Has_Artists : GetArtist_Tests
        {
            [SetUp]
            public override void SetUp()
            {
                base.SetUp();
                Result = XElementToTest.GetArtists();
            }

            [Test]
            public void Then_Return_Expected_Data()
            {
                Assert.IsNotEmpty(Result);
                Assert.AreEqual(Artists.Count, Result.Count);
                Assert.AreEqual("Kevin Childress", Result.First());
            }
        }

        public class When_Has_No_Artists : GetArtist_Tests
        {
            [SetUp]
            public override void SetUp()
            {
                Artists = new List<string>();
                base.SetUp();
                Result = XElementToTest.GetArtists();
            }

            [Test]
            public void Then_Return_Empty_List()
            {
                Assert.IsEmpty(Result);
            }
        }
    }
}