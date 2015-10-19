using System.Xml.Linq;
using BoardGameGeekApiClient.Helpers;
using NUnit.Framework;

namespace BoardGameGeekApiClient.Tests.UnitTests.BoardGameGeekApiClientHelper
{
    public abstract class GetRankingTests : BoardGameGeekApiClientHelper_Tests
    {
        public int Result { get; set; }

        protected abstract override void SetXElementToTest();

        class When_Item_Has_Rank : GetRankingTests
        {
            private int rank = 29;

            protected override void SetXElementToTest()
            {
                XElementToTest =
                    XDocument.Parse(
                        @"<ranks>" +
                            string.Format("<rank type='subtype' id='1' name='boardgame' friendlyname='Board Game Rank' value='{0}' bayesaverage='7.69943'/>",rank) +
                            "<rank type='family' id='5496' name='thematic' friendlyname='Thematic Rank' value='9' bayesaverage='7.69039'/>" +
                        "</ranks>").Root;
            }

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();
                Result = XElementToTest.GetRanking();
            }

            [Test]
            public void Then_BoardGameRank_Is_Returned()
            {
                Assert.AreEqual(rank, Result);
            }
        }

        class When_Item_Has_No_Rank : GetRankingTests
        {
            protected override void SetXElementToTest()
            {
                XElementToTest =
                                   XDocument.Parse(
                        @"<ranks>" +                            
                            "<rank type='family' id='5496' name='thematic' friendlyname='Thematic Rank' value='9' bayesaverage='7.69039'/>" +
                        "</ranks>").Root;
            }

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();
                Result = XElementToTest.GetRanking();
            }

            [Test]
            public void Then_Return_Minus_1()
            {
                Assert.AreEqual(-1, Result);
            }
        }

    }
}