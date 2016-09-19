using System.Collections.Generic;
using System.Linq;
using BoardGameGeekApiClient.Helpers;
using NUnit.Framework;
using BoardGameGeekApiClient.Models;

namespace BoardGameGeekApiClient.Tests.UnitTests.Helpers.BoardGameGeekApiClientHelper
{
    public class GetMechanics_Tests : GetTypedValues_BaseTest
    {
        public List<GameMechanic> Result { get; set; }

        public class When_Has_Mechanincs : GetMechanics_Tests
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

        public class When_Has_No_Mechanincs : GetMechanics_Tests
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