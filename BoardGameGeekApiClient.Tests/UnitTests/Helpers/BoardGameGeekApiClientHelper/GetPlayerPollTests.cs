using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using BoardGameGeekApiClient.Helpers;
using BoardGameGeekApiClient.Models;
using NUnit.Framework;

namespace BoardGameGeekApiClient.Tests.UnitTests.BoardGameGeekApiClientHelper
{
    public abstract class GetPlayerPollTests : BoardGameGeekApiClientHelper_Tests
    {
        public List<PlayerPollResult> Result { get; set; }

        protected abstract override void SetXElementToTest();

        class When_Item_Has_PollResults : GetPlayerPollTests
        {
            protected override void SetXElementToTest()
            {
                XElementToTest =
                    XDocument.Parse(
                        @"<poll name='suggested_numplayers' title='User Suggested Number of Players' totalvotes='769'>" +
                            "<results numplayers='1'>" +
                                "<result value='Best' numvotes='1'/>" +
                                "<result value='Recommended' numvotes='11'/>" +
                                "<result value='Not Recommended' numvotes='432'/>" +
                            "</results>" +
                            "<results numplayers='2'>" +
                                "<result value='Best' numvotes='2'/>" +
                                "<result value='Recommended' numvotes='10'/>" +
                                "<result value='Not Recommended' numvotes='438'/>" +
                            "</results>" +
                            "<results numplayers='3'>" +
                                "<result value='Best' numvotes='14'/>" +
                                "<result value='Recommended' numvotes='133'/>" +
                                "<result value='Not Recommended' numvotes='392' /> " +
                            "</results>" +
                            "<results numplayers='4'>" +
                                "<result value='Best' numvotes='48'/>" +
                                "<result value='Recommended' numvotes='425'/>" +
                                "<result value='Not Recommended' numvotes='145'/>" +
                            "</results>" +
                            "<results numplayers='5'>" +
                                "<result value='Best' numvotes='584'/>" +
                                "<result value='Recommended' numvotes='134'/>" +
                                "<result value='Not Recommended' numvotes='4'/>" +
                            "</results>" +
                            "<results numplayers='6'>" +
                                "<result value='Best' numvotes='244'/>" +
                                "<result value='Recommended' numvotes='366'/>" +
                                "<result value='Not Recommended' numvotes='49'/>" +
                            "</results>" +
                            "<results numplayers='6+'>" +
                                "<result value='Best' numvotes='21'/>" +
                                "<result value='Recommended' numvotes='86'/>" +
                                "<result value='Not Recommended' numvotes='223'/>" +
                            "</results>" +
                        "</poll>").Root;
            }

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();
                Result = XElementToTest.GetPlayerPollResults();
            }

            [Test]
            public void Then_PlayerPollResults_Is_Returned()
            {
                Assert.IsNotEmpty(Result);
                Assert.AreEqual(7, Result.Count);
                Assert.AreEqual(1, Result.First().Best);
                Assert.AreEqual(11, Result.First().Recommended);
                Assert.AreEqual(432, Result.First().NotRecommended);
            }
        }

        class When_Item_Has_No_Polls : GetPlayerPollTests
        {
            protected override void SetXElementToTest()
            {
                XElementToTest =
                   XDocument.Parse(
                       @"<poll name='suggested_numplayers' title='User Suggested Number of Players' totalvotes='769'>" +
                           
                       "</poll>").Root;
            }

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();
                Result = XElementToTest.GetPlayerPollResults();
            }

            [Test]
            public void Then_Return_Empty_Array()
            {
                Assert.IsEmpty(Result);
            }
        }

    }
}