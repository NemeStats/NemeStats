using NUnit.Framework;

namespace BoardGameGeekApiClient.Tests.IntegrationTests
{
    [TestFixture]
    public class SearchBoardGamesTests : BaseBoardGameGeekApiClientIntegrationTest
    {
        [Test]
        public void test()
        {
            var result = ApiClient.SearchBoardGames("alche").Result;
            Assert.IsNotNull(result);
        }
    }
}
