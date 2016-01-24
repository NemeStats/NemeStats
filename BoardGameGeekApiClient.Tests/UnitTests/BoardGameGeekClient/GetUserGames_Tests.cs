using System;
using System.Collections.Generic;
using BoardGameGeekApiClient.Interfaces;
using BoardGameGeekApiClient.Models;
using BoardGameGeekApiClient.Tests.Resources;
using NUnit.Framework;
using Rhino.Mocks;

namespace BoardGameGeekApiClient.Tests.UnitTests.BoardGameGeekClient
{
    public abstract class GetUserGames_Tests : BoardGameGeekClient_BaseTest
    {
        public IEnumerable<GameDetails> Result { get; set; }


        public class When_ApiDownload_Throw_Exception : GetUserGames_Tests
        {
            protected override IApiDownloadService CreateStub()
            {
                var stub = MockRepository.GenerateStub<IApiDownloadService>();
                stub.Stub(s => s.DownloadApiResult(Arg<Uri>.Is.Anything))
                    .Throw(new Exception());
                return stub;
            }


            [SetUp]
            public override void SetUp()
            {

                base.SetUp();

                Result = this.BoardGameGeekApiClient.GetUserGames("");

            }

            [Test]
            public void Then_Result_Is_Empty()
            {
                Assert.IsEmpty(Result);
            }
        }


        public class When_ApiDownload_Returns_No_Item : GetUserGames_Tests
        {


            [SetUp]
            public override void SetUp()
            {
                StubResponseString = "<not-an-item></not-an-item>";
                base.SetUp();

                Result = this.BoardGameGeekApiClient.GetUserGames("");
            }

            [Test]
            public void Then_Result_Is_Empty()
            {
                Assert.IsEmpty(Result);
            }
        }



        public class When_ApiDownload_Returns_Correct_Data : GetUserGames_Tests
        {


            [SetUp]
            public override void SetUp()
            {
                StubResponseString = TestsResources.GetUserGamesData;
                base.SetUp();

                Result = this.BoardGameGeekApiClient.GetUserGames("");
            }

            [Test]
            public void Then_Returns_A_Filled_List()
            {
                Assert.IsNotEmpty(Result);
            }
        }
    }
}