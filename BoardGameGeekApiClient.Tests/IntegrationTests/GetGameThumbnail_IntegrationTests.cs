using NUnit.Framework;

namespace BoardGameGeekApiClient.Tests.IntegrationTests
{
    public class GetGameThumbnail_IntegrationTests : BaseBoardGameGeekApiClientIntegrationTest
    {
        public string Result { get; set; }
        public int GameId { get; set; }

        public class When_GameId_Exists : GetGameThumbnail_IntegrationTests
        {


            [SetUp]
            public void SetUp()
            {
                GameId = 2536;
                Result = ApiClient.GetGameThumbnail(GameId);
            }

            [Test]
            public void Then_Return_Thumbnaiul()
            {
                Assert.IsNotNull(Result);                
            }


        }

        public class When_GameId_Not_Exists : GetGameThumbnail_IntegrationTests
        {


            [SetUp]
            public void SetUp()
            {
                GameId = int.MaxValue;
                Result = ApiClient.GetGameThumbnail(GameId);
            }

            [Test]
            public void Then_Return_Empty()
            {
                Assert.IsEmpty(Result);
            }

        }
    }
}