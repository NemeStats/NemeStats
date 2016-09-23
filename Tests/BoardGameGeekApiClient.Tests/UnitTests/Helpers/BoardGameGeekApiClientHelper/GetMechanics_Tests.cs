using BoardGameGeekApiClient.Helpers;
using BoardGameGeekApiClient.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

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
                var firstResult = Result.First();
                Assert.AreEqual("Area Movement", firstResult.Mechanic);
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