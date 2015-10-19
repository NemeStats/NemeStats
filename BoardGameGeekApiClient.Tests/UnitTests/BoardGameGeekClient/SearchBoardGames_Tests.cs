using System;
using System.Collections.Generic;
using BoardGameGeekApiClient.Interfaces;
using BoardGameGeekApiClient.Models;
using BoardGameGeekApiClient.Tests.Resources;
using NUnit.Framework;
using Rhino.Mocks;

namespace BoardGameGeekApiClient.Tests.UnitTests.BoardGameGeekClient
{
    public abstract class SearchBoardGames_Tests : BoardGameGeekClient_BaseTest
    {
        public IEnumerable<SearchBoardGameResult> Result { get; set; }


        public class When_ApiDownload_Throw_Exception : SearchBoardGames_Tests
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

                Result = this.BoardGameGeekApiClient.SearchBoardGames("");

            }

            [Test]
            public void Then_Result_Is_Empty()
            {
                Assert.IsEmpty(Result);
            }
        }


        public class When_ApiDownload_Returns_No_Item : SearchBoardGames_Tests
        {


            [SetUp]
            public override void SetUp()
            {
                StubResponseString = "<not-an-item></not-an-item>";
                base.SetUp();

                Result = this.BoardGameGeekApiClient.SearchBoardGames("");
            }

            [Test]
            public void Then_Result_Is_Empty()
            {
                Assert.IsEmpty(Result);
            }
        }



        public class When_ApiDownload_Returns_Correct_Data : SearchBoardGames_Tests
        {


            [SetUp]
            public override void SetUp()
            {
                StubResponseString = TestsResources.SearchBoardGamesData;
                base.SetUp();

                Result = this.BoardGameGeekApiClient.SearchBoardGames("");
            }

            [Test]
            public void Then_Returns_A_Filled_List()
            {
                Assert.IsNotEmpty(Result);
            }
        }
    }
}