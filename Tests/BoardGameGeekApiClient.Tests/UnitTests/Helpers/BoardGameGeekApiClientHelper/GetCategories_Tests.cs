using System.Collections.Generic;
using System.Linq;
using BoardGameGeekApiClient.Helpers;
using NUnit.Framework;
using BoardGameGeekApiClient.Models;

namespace BoardGameGeekApiClient.Tests.UnitTests.Helpers.BoardGameGeekApiClientHelper
{
    public class GetCategories_Tests : GetTypedValues_BaseTest
    {
        public List<GameCategory> Result { get; set; }

        public class When_Has_Categoriess : GetCategories_Tests
        {
            [SetUp]
            public override void SetUp()
            {
                base.SetUp();
                Result = XElementToTest.GetCategories();
            }

            [Test]
            public void Then_Return_Expected_Data()
            {
                Assert.IsNotEmpty(Result);
                Assert.AreEqual(Categories.Count, Result.Count);
                Assert.AreEqual("Bluffing", Result.First());
            }
        }

        public class When_Has_No_Categories : GetCategories_Tests
        {
            [SetUp]
            public override void SetUp()
            {
                Categories = new List<string>();
                base.SetUp();
                Result = XElementToTest.GetCategories();
            }

            [Test]
            public void Then_Return_Empty_List()
            {
                Assert.IsEmpty(Result);
            }
        }
    }
}