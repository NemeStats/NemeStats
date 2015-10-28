using System;
using BoardGameGeekApiClient.Interfaces;
using BoardGameGeekApiClient.Models;
using BoardGameGeekApiClient.Tests.Resources;
using NUnit.Framework;
using Rhino.Mocks;

namespace BoardGameGeekApiClient.Tests.UnitTests.BoardGameGeekClient
{
    public abstract class GetGameDetails_Tests : BoardGameGeekClient_BaseTest
    {
        public GameDetails Result { get; set; }

        public class When_ApiDownload_Throw_Exception : GetGameDetails_Tests
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

                Result = this.BoardGameGeekApiClient.GetGameDetails(1);
            }

            [Test]
            public void Then_Result_Is_Null()
            {
                Assert.IsNull(Result);
            }
        }


        public class When_ApiDownload_Returns_No_Item : GetGameDetails_Tests
        {


            [SetUp]
            public override void SetUp()
            {
                StubResponseString = "<not-an-item></not-an-item>";
                base.SetUp();

                Result = this.BoardGameGeekApiClient.GetGameDetails(1);
            }

            [Test]
            public void Then_Result_Is_Null()
            {
                Assert.IsNull(Result);
            }
        }

        public class When_ApiDownload_Returns_More_Than_One_Item : GetGameDetails_Tests
        {


            [SetUp]
            public override void SetUp()
            {
                StubResponseString = "<items><item></item><item></item></items>";
                base.SetUp();

                Result = this.BoardGameGeekApiClient.GetGameDetails(1);
            }

            [Test]
            public void Then_Result_Is_Null()
            {
                Assert.IsNull(Result);
            }
        }

        public class When_ApiDownload_Returns_Wrong_Data : GetGameDetails_Tests
        {


            [SetUp]
            public override void SetUp()
            {
                StubResponseString = "<items><item></item></items>";
                base.SetUp();

                Result = this.BoardGameGeekApiClient.GetGameDetails(1);
            }

            [Test]
            public void Then_Result_Is_Null()
            {
                Assert.IsNull(Result);
            }
        }

        public class When_ApiDownload_Returns_Correct_Data : GetGameDetails_Tests
        {


            [SetUp]
            public override void SetUp()
            {
                StubResponseString = TestsResources.GetGameDetailsData;
                base.SetUp();

                Result = this.BoardGameGeekApiClient.GetGameDetails(1);
            }

            [Test]
            public void Then_Returns_Filled_Object()
            {
                Assert.IsNotNull(Result);
            }
        }
    }
}

