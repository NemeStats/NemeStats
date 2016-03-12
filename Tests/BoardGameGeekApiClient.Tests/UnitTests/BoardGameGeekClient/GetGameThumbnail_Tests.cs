using System;
using BoardGameGeekApiClient.Interfaces;
using BoardGameGeekApiClient.Tests.Resources;
using NUnit.Framework;
using Rhino.Mocks;

namespace BoardGameGeekApiClient.Tests.UnitTests.BoardGameGeekClient
{
    public abstract class GetGameThumbnail_Tests : BoardGameGeekClient_BaseTest
    {
        public string Result { get; set; }

        public class When_ApiDownload_Throw_Exception : GetGameThumbnail_Tests
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

                Result = this.BoardGameGeekApiClient.GetGameThumbnail(1);
            }

            [Test]
            public void Then_Result_Is_Null()
            {
                Assert.IsEmpty(Result);
            }
        }


        public class When_ApiDownload_Returns_No_Item : GetGameThumbnail_Tests
        {


            [SetUp]
            public override void SetUp()
            {
                StubResponseString = "<not-an-item></not-an-item>";
                base.SetUp();

                Result = this.BoardGameGeekApiClient.GetGameThumbnail(1);
            }

            [Test]
            public void Then_Result_Is_Null()
            {
                Assert.IsEmpty(Result);
            }
        }

        public class When_ApiDownload_Returns_More_Than_One_Item : GetGameThumbnail_Tests
        {


            [SetUp]
            public override void SetUp()
            {
                StubResponseString = "<items><item></item><item></item></items>";
                base.SetUp();

                Result = this.BoardGameGeekApiClient.GetGameThumbnail(1);
            }

            [Test]
            public void Then_Result_Is_Null()
            {
                Assert.IsEmpty(Result);
            }
        }

        public class When_ApiDownload_Returns_Wrong_Data : GetGameThumbnail_Tests
        {


            [SetUp]
            public override void SetUp()
            {
                StubResponseString = "<items><item></item></items>";
                base.SetUp();

                Result = this.BoardGameGeekApiClient.GetGameThumbnail(1);
            }

            [Test]
            public void Then_Result_Is_Null()
            {
                Assert.IsEmpty(Result);
            }
        }

        public class When_ApiDownload_Returns_Correct_Data : GetGameThumbnail_Tests
        {


            [SetUp]
            public override void SetUp()
            {
                StubResponseString = TestsResources.GetUserGamesData;
                base.SetUp();

                Result = this.BoardGameGeekApiClient.GetGameThumbnail(1);
            }

            [Test]
            public void Then_Returns_Filled_Object()
            {
                Assert.IsNotEmpty(Result);
            }
        }
    }
}